using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using EHS.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EHS.Infrastructure.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JWTOptions> jwtOptions,
        ILogger<AuthService> logger
        ) : IAuthService
    {
        private const int MaxFailedAttempts = 5;

        private const int LockoutMinutes = 15;

        private const int MaxActiveRefreshTokens = 5;

        private readonly JWTOptions _jwtOptions = jwtOptions.Value;

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(loginRequest.Email);

                // Check if user exists
                if (user == null)
                {
                    logger.LogWarning("Login attempt for non-existing user email: {Email}", loginRequest.Email);
                    return new AuthResponse { Message = "Invalid email or password" };
                }

                // Check if user is locked out
                if (await userManager.IsLockedOutAsync(user))
                {
                    logger.LogWarning("Locked out user attempted login: {Email}", loginRequest.Email);
                    return new AuthResponse { Message = $"Account is locked. Please try again after {user.LockoutEnd?.Subtract(DateTimeOffset.UtcNow).Minutes} minutes." };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    logger.LogWarning("Inactive user attempted login: {Email}", loginRequest.Email);
                    return new AuthResponse { Message = "Account is inactive. Please contact support." };
                }

                if (!await userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    // Increment failed access count
                    await userManager.AccessFailedAsync(user);

                    var failedAttempts = await userManager.GetAccessFailedCountAsync(user);
                    logger.LogWarning("Invalid password attempt {FailedAttempts} for user email: {Email}", failedAttempts, loginRequest.Email);

                    // Lock the user if max failed attempts reached
                    if (failedAttempts >= MaxFailedAttempts)
                    {
                        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(LockoutMinutes));
                        logger.LogWarning("User account locked due to multiple failed login attempts: {Email}", loginRequest.Email);
                        return new AuthResponse { Message = $"Account locked due to multiple failed login attempts. Please try again after {LockoutMinutes} minutes." };
                    }

                    return new AuthResponse { Message = "Invalid password." };
                }

                // Reset failed access count on successful login
                await userManager.ResetAccessFailedCountAsync(user);

                // Clean up expired refresh tokens before creating new one
                await CleanUpExpiredTokensAsync(user);

                // Limit concurrent sessions
                await EnforceRefreshTokenLimitAsync(user);

                user.LastLoginAt = DateTime.UtcNow;
                var jwtToken = await CreateJwtTokenAsync(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshTokens ??= [];
                user.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(user);

                var roles = (await userManager.GetRolesAsync(user)).ToList();

                logger.LogInformation("User logged in successfully: {Email}", loginRequest.Email);

                return new AuthResponse()
                {
                    IsAuthenticated = true,
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn,
                    Username = user!.UserName!,
                    Email = user.Email!,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during login for email: {Email}", loginRequest.Email);
                return new AuthResponse { Message = "An error occurred during login. Please try again later." };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                if (await userManager.FindByEmailAsync(registerRequest.Email) != null)
                {
                    logger.LogWarning("Registration attempt with existing email: {Email}", registerRequest.Email);
                    return new AuthResponse { Message = "User with this email already exists." };
                }

                if (await userManager.FindByNameAsync(registerRequest.UserName) != null)
                {
                    logger.LogWarning("Registration attempt with existing username: {UserName}", registerRequest.UserName);
                    return new AuthResponse { Message = "User with this username already exists." };
                }

                var user = new ApplicationUser
                {
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    UserName = registerRequest.UserName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, registerRequest.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning("User registration failed for {Email}: {Errors}", registerRequest.Email, errors);
                    return new AuthResponse { Message = errors };
                }

                if (!(await roleManager.RoleExistsAsync(registerRequest.Role)))
                {
                    var roleCreateResult = await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = registerRequest.Role,
                    });

                    if (!roleCreateResult.Succeeded)
                    {
                        logger.LogWarning("User registered successfully but failed to create new role. Email: {Email}, Role: {Role}", registerRequest.Email, registerRequest.Role);
                        return new AuthResponse
                        {
                            IsAuthenticated = true,
                            Message = "User registered successfully but failed to create new role."
                        };
                    }
                }

                await userManager.AddToRoleAsync(user, registerRequest.Role);

                logger.LogInformation("User registered successfully: {UserId}", user.Id);

                return new AuthResponse
                {
                    IsAuthenticated = true,
                    Message = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during registration for email: {Email}", registerRequest.Email);
                return new AuthResponse { Message = "An error occurred during registration. Please try again." };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthResponse { Message = "Invalid refresh token" };
                }

                var user = await userManager.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.RefreshTokens != null
                    && u.RefreshTokens.Any(t => t.Token == token));

                if (user == null)
                {
                    logger.LogWarning("Refresh token not found: {Token}", token[..10] + "...");
                    return new AuthResponse { Message = "Invalid refresh token" };
                }

                var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);

                if (refreshToken is null)
                {
                    logger.LogWarning("Refresh token not found for user: {UserId}", user.Id);
                    return new AuthResponse { Message = "Invalid refresh token" };
                }

                // If revoked token is used, revoke the entire token family
                if (refreshToken.RevokedOn.HasValue)
                {
                    logger.LogWarning("Attempted reuse of revoked refresh token by user: {UserId}. Revoking all tokens.", user.Id);
                    await RevokeAllUserTokensAsync(user);
                    return new AuthResponse { Message = "Invalid refresh token. All sessions have been terminated for security." };
                }

                if (!refreshToken.IsActive)
                {
                    logger.LogWarning("Expired refresh token used by user: {UserId}", user.Id);
                    return new AuthResponse { Message = "Refresh token expired. Please login again." };
                }

                if (!user.IsActive)
                {
                    logger.LogWarning("Refresh token attempt for inactive account: {UserId}", user.Id);
                    return new AuthResponse { Message = "Account is inactive." };
                }

                refreshToken.RevokedOn = DateTime.UtcNow;

                var newRefreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);

                // Clean up old tokens
                await CleanUpExpiredTokensAsync(user);

                await userManager.UpdateAsync(user);

                var jwtToken = await CreateJwtTokenAsync(user);
                var roles = (await userManager.GetRolesAsync(user)).ToList();

                logger.LogInformation("Refresh token rotated for user: {UserId}", user.Id);
                return new AuthResponse
                {
                    IsAuthenticated = true,
                    Token = jwtToken,
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during token refresh");
                return new AuthResponse { Message = "An error occurred during token refresh. Please login again." };
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return false;
                }

                var user = await userManager.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

                if (user == null)
                {
                    logger.LogWarning("Revoke attempt for non-existent token");
                    return false;
                }

                var refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == token);

                if (refreshToken == null || !refreshToken.IsActive)
                {
                    logger.LogWarning("Revoke attempt for inactive token by user: {UserId}", user.Id);
                    return false;
                }

                refreshToken.RevokedOn = DateTime.UtcNow;
                await userManager.UpdateAsync(user);

                logger.LogInformation("Refresh token revoked for user: {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during token revocation.");
                return false;
            }
        }

        private async Task<string> CreateJwtTokenAsync(ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new List<Claim> {
                // Standard identity claims
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),

                // JWT standard claims
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var jwtSecuritytoken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMin),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigingKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecuritytoken);

            return token;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpirationInMin),
                CreatedOn = DateTime.UtcNow,
            };
        }

        private async Task CleanUpExpiredTokensAsync(ApplicationUser user)
        {
            if (user.RefreshTokens is null)
            {
                return;
            }

            var expiredTokens = user.RefreshTokens
                .Where(t => !t.IsActive)
                .ToList();

            if (expiredTokens.Count != 0)
            {
                foreach (var token in expiredTokens)
                {
                    user.RefreshTokens.Remove(token);
                }

                logger.LogInformation("Cleaned up {Count} expired refresh tokens for user: {UserId}", expiredTokens.Count, user.Id);
            }
        }

        private async Task EnforceRefreshTokenLimitAsync(ApplicationUser user)
        {
            if (user.RefreshTokens is null)
            {
                return;
            }

            var activeTokens = user.RefreshTokens
                .Where(t => t.IsActive)
                .OrderBy(t => t.CreatedOn)
                .ToList();

            if (activeTokens.Count > MaxActiveRefreshTokens)
            {
                var tokensToRevoke = activeTokens.Take(activeTokens.Count - MaxActiveRefreshTokens + 1);
                foreach (var token in tokensToRevoke)
                {
                    token.RevokedOn = DateTime.UtcNow;
                }
            }
        }

        public async Task<bool> RevokeAllUserTokensAsync(ApplicationUser user)
        {
            try
            {
                if (user.RefreshTokens == null || !user.RefreshTokens.Any())
                {
                    return true;
                }

                foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
                {
                    token.RevokedOn = DateTime.UtcNow;
                }

                await userManager.UpdateAsync(user);
                logger.LogInformation("All tokens revoked for user: {UserId}.", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error revoking all tokens for user: {UserId}", user.Id);
                return false;
            }
        }
    }
}
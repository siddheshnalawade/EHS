using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using EHS.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        IOptions<JWTOptions> jwtOptions
        ) : IAuthService
    {
        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null || !(await userManager.CheckPasswordAsync(user, loginRequest.Password)))
            {
                return new AuthResponse() { Message = "Invalid email or password" };
            }

            var jwtToken = await CreateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens ??= new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);

            var roles = (await userManager.GetRolesAsync(user)).ToList();

            return new AuthResponse()
            {
                IsAuthenticated = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await userManager.FindByEmailAsync(registerRequest.Email) != null)
            {
                return new AuthResponse { Message = "User with this email already exists." };
            }

            var user = new ApplicationUser()
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
            };

            var result = await userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponse { Message = errors };
            }

            if (!(await roleManager.RoleExistsAsync(registerRequest.Role)))
            {
                await roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = registerRequest.Role,
                });
            }

            await userManager.AddToRoleAsync(user, "Initiator");

            return new AuthResponse { IsAuthenticated = true, Message = "User registered successfully." };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.RefreshTokens != null
            && u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                return new AuthResponse() { Message = "Invalid refresh token" };
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                return new AuthResponse() { Message = "Refresh token expired." };
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtTokenAsync(user);
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            return new AuthResponse()
            {
                IsAuthenticated = true,
                Token = jwtToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                Roles = roles
            };
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            return true;
        }

        private async Task<string> CreateJwtTokenAsync(ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new List<Claim> {
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = jwtOptions.Value;

            var authSigingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            var jwtSecuritytoken = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                expires: DateTime.UtcNow.AddMinutes(jwt.ExpirationInMin),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigingKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecuritytoken);

            return token;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var jwt = jwtOptions.Value;

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddMinutes(jwt.RefreshTokenExpirationInMin),
                CreatedOn = DateTime.UtcNow,
            };
        }
    }
}
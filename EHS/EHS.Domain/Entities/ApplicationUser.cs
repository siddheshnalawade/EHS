using Microsoft.AspNetCore.Identity;

namespace EHS.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
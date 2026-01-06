using System.ComponentModel.DataAnnotations;

namespace EHS.Application.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Emain is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 256 characters.")]
        public string Password { get; set; }
    }
}
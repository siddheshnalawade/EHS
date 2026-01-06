using System.ComponentModel.DataAnnotations;

namespace EHS.Application.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(256, ErrorMessage = "Full name cannot exceed 256 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]*$", ErrorMessage = "Full name contains invalid characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(128, ErrorMessage = "Username cannot exceed 128 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "Username contains invalid characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 256 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(128, ErrorMessage = "Role cannot exceed 128 characters")]
        public string Role { get; set; } = "Initiator";
    }
}
using System.ComponentModel.DataAnnotations;

namespace TodoApp.DTOs.User
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Please Enter Username.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter EmailAddress.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
        public string Password { get; set; }
    }
}

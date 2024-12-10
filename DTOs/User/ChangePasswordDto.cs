using System.ComponentModel.DataAnnotations;

namespace TodoApp.DTOs.User
{
    public class ChangePasswordDto
    {

        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
    }
}

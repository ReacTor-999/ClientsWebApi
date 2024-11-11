using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Users
{
    public record UserUpdatePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}

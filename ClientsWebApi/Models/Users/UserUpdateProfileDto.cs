using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Users
{
    public record UserUpdateProfileDto
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

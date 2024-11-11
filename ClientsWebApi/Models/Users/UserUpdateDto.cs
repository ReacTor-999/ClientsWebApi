using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public record UserUpdateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string[] RolesToAdd { get; set; } = { };
        public string[] RolesToRemove { get; set; } = { };
    }
}

using ClientsWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [NonEmptyEnumeration]
        public List<string> Roles { get; set; } = new();
    }
}

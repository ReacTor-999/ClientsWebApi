using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.ClientProfile
{
    public record ClientProfileUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}

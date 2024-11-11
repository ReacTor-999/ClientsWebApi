using ClientsWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.ContactInfo
{
    public record ContactInfoCreateDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Other { get; set; }
    }
}

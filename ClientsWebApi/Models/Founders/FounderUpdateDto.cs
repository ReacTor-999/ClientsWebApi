using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public record FounderUpdateDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? Patronymic { get; set; }

    }
}

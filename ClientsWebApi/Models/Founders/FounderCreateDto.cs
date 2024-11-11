using ClientsWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public record FounderCreateDto
    {
        [Required]
        [TaxpayerIndividualNumber]
        [MinLength(12)]
        public string TIN { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? Patronymic { get; set; }
    }
}

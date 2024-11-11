using ClientsWebApi.Entities;
using ClientsWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Clients
{
    public record ClientCreateDto
    {
        [Required]
        [TaxpayerIndividualNumber]
        public string TIN { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        public string Type { get; set; } = string.Empty;

        [NonEmptyEnumeration]
        public List<int> FounderIds { get; set; } = new();
    }
}

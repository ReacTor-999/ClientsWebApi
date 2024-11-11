using ClientsWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Account
{
    public record AccountVerifyDto
    {
        [Required]
        [TaxpayerIndividualNumber]
        public string TaxpayerIndividualNumber { get; set; } = string.Empty;
    }
}

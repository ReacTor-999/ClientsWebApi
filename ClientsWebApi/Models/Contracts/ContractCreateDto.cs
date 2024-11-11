using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Contracts
{
    public class ContractCreateDto
    {
        [Required]
        public DateTime ConclusionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        public double Amount { get; set; }

        public bool IsProvided { get; set; } = false;

        public bool IsPaid { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientsWebApi.Entities
{
    public class Payment: EntityBase
    {
        [Key]
        public int Id { get; set; }

        public int ContractId { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        public double Amount { get; set; }

        public DateTime PaymentTime { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; } = null!;
    }
}

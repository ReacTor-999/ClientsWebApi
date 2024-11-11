using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientsWebApi.Entities
{
    public class Contract: EntityBase
    {
        [Key]
        public int Id { get; set; }

        public int? ClientId { get; set; }

        [Column(TypeName = "VARCHAR"), MaxLength(12)]
        public string ClientTIN { get; set; } = string.Empty;

        public DateTime ConclusionDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }

        public double Amount { get; set; }

        public bool IsProvided { get; set; }

        public bool IsPaid { get; set; }


        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; } = null!;
    }
}

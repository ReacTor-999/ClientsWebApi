using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientsWebApi.Entities
{
    public class ContactInfo : EntityBase
    {
        [Key]
        public int Id { get; set; }

        
        public int ClientId { get; set; }


        [Column(TypeName = "VARCHAR"), MaxLength(15)]
        public string? PhoneNumber { get; set; }


        [Column(TypeName = "VARCHAR"), MaxLength(255)]
        public string? Email { get; set; }


        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;


        [MaxLength(255)]
        public string? Other { get; set; }


        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
    }
}

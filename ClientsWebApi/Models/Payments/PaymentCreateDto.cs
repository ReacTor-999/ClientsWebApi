using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Dto
{
    public class PaymentCreateDto
    {
        [Required]
        public double Amount { get; set; }

        public string? Description { get; set; }
    }
}

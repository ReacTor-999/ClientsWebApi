using ClientsWebApi.Entities;

namespace ClientsWebApi.Models.Dto
{
    public class PaymentGetDto
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public double Amount { get; set; }

        public string? Description { get; set; }

        public DateTime PaymentTime { get; set; }

        public PaymentGetDto(Payment payment)
        {
            Id = payment.Id;
            ContractId = payment.ContractId;
            Amount = payment.Amount;
            Description = payment.Description;
            PaymentTime = payment.PaymentTime;
        }
    }
}

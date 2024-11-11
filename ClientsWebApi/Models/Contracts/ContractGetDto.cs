using ClientsWebApi.Entities;

namespace ClientsWebApi.Models.Contracts
{
    public record ContractGetDto
    {
        public int Id { get; set; }

        public string ClientTIN { get; set; } = string.Empty;

        public int? ClientId { get; set; }

        public DateTime ConclusionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public double Amount { get; set; }

        public bool IsProvided { get; set; }

        public bool IsPaid { get; set; }

        public ContractGetDto(Contract contract)
        {
            Id = contract.Id;
            ClientTIN = contract.ClientTIN;
            ClientId = contract.ClientId;
            ConclusionDate = contract.ConclusionDate;
            ExpirationDate = contract.ExpirationDate;
            Amount = contract.Amount;
            IsProvided = contract.IsProvided;
            IsPaid = contract.IsPaid;
        }
    }
}

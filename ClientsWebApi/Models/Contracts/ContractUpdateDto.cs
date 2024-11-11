namespace ClientsWebApi.Models.Contracts
{
    public record ContractUpdateDto
    {
        public DateTime ConclusionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public double Amount { get; set; }

        public bool IsProvided { get; set; } = false;

        public bool IsPaid { get; set; } = false;
    }
}

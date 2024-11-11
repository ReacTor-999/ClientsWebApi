using ClientsWebApi.Entities;

namespace ClientsWebApi.Models.Founders
{
    public record FounderGetDto
    {
        public int Id { get; set; }

        public string TIN { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Patronymic { get; set; }

        public FounderGetDto(Founder founder)
        {
            Id = founder.Id;
            TIN = founder.TaxpayerIndividualNumber;
            FirstName = founder.FirstName;
            LastName = founder.LastName;
            Patronymic = founder.Patronymic;
        }
    }
}

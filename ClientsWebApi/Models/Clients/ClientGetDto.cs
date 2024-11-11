using ClientsWebApi.Entities;

namespace ClientsWebApi.Models.Clients
{
    public class ClientGetDto
    {
        public int Id { get; set; }

        public string TIN { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public List<int> FounderIds { get; set; } = new();

        public ClientGetDto(Client client, IEnumerable<Founder> founders)
        {
            Id = client.Id;
            TIN = client.TaxpayerIndividualNumber;
            Name = client.Name;
            Type = client.Type;

            founders ??= Enumerable.Empty<Founder>();

            FounderIds = new(founders.Select(f => f.Id));
        }
    }
}

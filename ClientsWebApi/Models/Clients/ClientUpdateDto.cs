using ClientsWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Models.Clients
{
    public class ClientUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [NonEmptyEnumeration]
        public List<int> FounderIds { get; set; } = new();
    }
}

using Microsoft.AspNetCore.Identity;

namespace ClientsWebApi.Models.Dto
{
    public record UserGetDto
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new();

        public UserGetDto(IdentityUser user, IEnumerable<IdentityRole> roles)
        {
            Id = user.Id;
            Email = user.Email;

            Roles = new(roles.ToList().Select(r => r.Name));
        }
    }
}

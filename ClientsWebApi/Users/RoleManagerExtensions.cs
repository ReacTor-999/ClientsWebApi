using Microsoft.AspNetCore.Identity;

namespace ClientsWebApi.Users
{
    public static class RoleManagerExtensions
    {
        public static async Task<IdentityRole> FindOrCreateAsync(this RoleManager<IdentityRole> roleManager, UserRole userRole)
        {
            var roleName = Enum.GetName(userRole);

            return await roleManager.FindOrCreateAsync(roleName);
        }

        public static async Task<IdentityRole> FindOrCreateAsync(this RoleManager<IdentityRole> roleManager, string? roleName)
        {
            var identityRole = await roleManager.FindByNameAsync(roleName);

            if (identityRole == null)
            {
                identityRole = new IdentityRole(roleName);
                await roleManager.CreateAsync(identityRole);
            }

            return identityRole;
        }
    }
}

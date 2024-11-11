using ClientsWebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Claims;

namespace ClientsWebApi.Controllers
{
    public static class ControllerBaseExtensions
    {
        public static async Task<TUser> GetAuthorizedUserAsync<TUser>(this ControllerBase controller, UserManager<TUser> userManager) where TUser : IdentityUser
        {
            var email = controller.HttpContext.User.Claims
                                                   .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                                                   .Value;

            return await userManager.FindByEmailAsync(email);
        }


        public static async Task<IActionResult> GetDbSaveResultAsync(this ControllerBase controller, DbContext ctx)
        {
            var result = await ctx.TrySaveChangesAsync();

            if (result.IsSuccess)
                return controller.Ok(result.Message);
            else
                return controller.BadRequest(result.Message);
        }
    }
}

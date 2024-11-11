using ClientsWebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClientsWebApi.Models.Users;
using ClientsWebApi.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClientsWebApi.Controllers
{
    [ApiController]
    [Route("api/user/profile")]
    [Authorize]
    [SwaggerTag("Provides some operation for all users to get or modify their accounts")]
    public class ProfileController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        [Produces(typeof(UserGetDto))]
        [SwaggerOperation(
            Summary = "Allows to get the authorized user's account information",
            Description = "Requires autorized user",
            OperationId = "UserGetAccountInfo"
        )]
        public async Task<IActionResult> GetAccountInfo()
        {
            var user = await this.GetAuthorizedUserAsync(_userManager);
            if (user == null)
            {
                return BadRequest();
            }

            var roles = _dbContext.UserRoles
                                  .AsNoTracking()
                                  .Where(ur => ur.UserId == user.Id)
                                  .Join(_dbContext.Roles,
                                        ur => ur.RoleId, role => role.Id,
                                        (ur, role) => role)
                                  .ToList();

            return Ok(new UserGetDto(user, roles));
        }


        [HttpPut]
        [SwaggerOperation(
            Summary = "Allows to update the authorized user's account",
            Description = "Requires autorized user",
            OperationId = "UserUpdateAccountInfo"
        )]
        public async Task<IActionResult> UpdateAccountInfo(UserUpdateProfileDto newUser)
        {
            var user = await this.GetAuthorizedUserAsync(_userManager);
            if (user == null)
            {
                return BadRequest();
            }

            var entry = _dbContext.Attach(user);

            user.Email = newUser.Email;

            entry.Property(e => e.Email).IsModified = true;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpPut("password")]
        [SwaggerOperation(
            Summary = "Allows to change the authorized user's password",
            Description = "Requires autorized user.",
            OperationId = "UserChangePassword"
        )]
        public async Task<IActionResult> ChangePassword(UserUpdatePasswordDto newPassword)
        {
            var user = await this.GetAuthorizedUserAsync(_userManager);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ChangePasswordAsync(user, newPassword.OldPassword, newPassword.NewPassword);

            if (result.Succeeded == false)
            {
                return BadRequest(result.Errors);
            }

            return await this.GetDbSaveResultAsync(_dbContext);
        }
    }
}

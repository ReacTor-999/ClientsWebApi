using ClientsWebApi.Entities;
using ClientsWebApi.Models.Dto;
using ClientsWebApi.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace ClientsWebApi.Controllers
{
    [ApiController]
    [Route("/api/users")]
    [Authorize(Roles = "Admin")]
    [SwaggerTag("Provides some operations for admins to manipulate user accounts")]
    public class UsersManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersManagementController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        [Produces(typeof(UserGetDto[]))]
        [SwaggerOperation(
            Summary = "Allows to get users with their roles",
            Description = "Requires admin priviliges",
            OperationId = "ListUsers"
        )]
        public IActionResult ListUsers()
        {
            var usersRoles = _dbContext.Users
                                       .AsNoTracking()
                                       .Join(_dbContext.UserRoles,
                                             user => user.Id, ur => ur.UserId,
                                             (user, ur) => new { user, ur })
                                       .Join(_dbContext.Roles,
                                             ur => ur.ur.RoleId, role => role.Id,
                                             (ur, role) => new { ur.user, role });

            var result = usersRoles.ToList()
                                   .GroupBy(ur => ur.user.Id)
                                   .Select(grouping => 
                                           grouping.Aggregate(new List<IdentityRole>().AsEnumerable(),
                                                              (roles, ur) => roles.Append(ur.role),
                                                              (roles) => new UserGetDto(grouping.First().user, roles)
                                   ));


            return Ok(result);
        }


        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new user",
            Description = "Requires admin priviliges",
            OperationId = "CreateUser"
        )]
        public async Task<IActionResult> CreateUser(UserCreateDto user)
        {
            var rolesToAdd = user.Roles.Intersect(Enum.GetNames<UserRole>(), StringComparer.OrdinalIgnoreCase);

            foreach (var roleNmae in rolesToAdd)
            {
                await _roleManager.FindOrCreateAsync(roleNmae);
            }

            var created = new IdentityUser()
            {
                Email = user.Email,
                UserName = user.Email.Substring(user.Email.IndexOf('@')),
            };

            var result = await _userManager.CreateAsync(created);
            if (result.Succeeded == false)
            {
                return Conflict(result.Errors);
            }

            result = await _userManager.AddToRolesAsync(created, rolesToAdd);
            if (result.Succeeded == false)
            {
                return Conflict(result.Errors);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("{userId}")]
        [SwaggerOperation(
            Summary = "Allows to delete a specified user",
            Description = "Requires admin priviliges",
            OperationId = "DeleteUser"
        )]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            try
            {
                await _userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }


        [HttpPut("{userId}")]
        [SwaggerOperation(
            Summary = "Updates an existing user",
            Description = "Requires admin priviliges",
            OperationId = "UpdateUser"
        )]
        public async Task<IActionResult> UpdateUser(string userId, UserUpdateDto userUpdate)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user.Email = userUpdate.Email;
            user.UserName = userUpdate.Email.Substring(userUpdate.Email.IndexOf('@'));

            await _userManager.UpdateAsync(user);

            var rolesToAdd = userUpdate.RolesToAdd.Intersect(Enum.GetNames<UserRole>(), StringComparer.OrdinalIgnoreCase);

            foreach (var roleNmae in rolesToAdd)
            {
                await _roleManager.FindOrCreateAsync(roleNmae);
            }

            try
            {
                await _userManager.RemoveFromRolesAsync(user, userUpdate.RolesToRemove);
                await _userManager.AddToRolesAsync(user, rolesToAdd);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

            return Ok();
        }
    }
}

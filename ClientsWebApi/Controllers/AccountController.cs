using ClientsWebApi.Entities;
using ClientsWebApi.Models.Account;
using ClientsWebApi.Models.Dto;
using ClientsWebApi.Services;
using ClientsWebApi.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace ClientsWebApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtService _jwtService;

        public AccountController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, JwtService jwtService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }


        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), 400)]
        public async Task<IActionResult> Register(AccountRegisterDto request)
        {
            var identity = new IdentityUser()
            {
                Email = request.Email,
                UserName = new(request.Email.TakeWhile(ch => ch != '@').ToArray())
            };

            var result = await _userManager.CreateAsync(identity, request.Password);
            if(result.Succeeded == false)
            {
                return BadRequest(result.Errors);
            }

            var role = await _roleManager.FindOrCreateAsync(UserRole.User);
            await _userManager.AddToRoleAsync(identity, role.Name);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(AccountAuthDto), 200)]
        public async Task<IActionResult> Login(AccountLoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null)
            {
                return BadRequest();
            }


            var result = _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if(result.IsCompletedSuccessfully == false)
            {
                return BadRequest();
            }


            var claims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Email, user.Email)
            });

            claimsIdentity.AddClaims(claims);

            foreach(var role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = _jwtService.CreateSecurityToken(claimsIdentity);
            var response = new AccountAuthDto()
            {
                Token = _jwtService.WriteToken(token)
            };

            return Ok(response);
        }


        //Верификация чтобы получить роль "Client". Сделано просто и неправильно, чисто для примера
        [Authorize(Roles = "User")]
        [HttpPut("verify")]
        public async Task<IActionResult> VerifyUser(AccountVerifyDto request)
        {
            var user = await this.GetAuthorizedUserAsync(_userManager);
            if (user == null)
            {
                return BadRequest();
            }

            var client = _dbContext.Clients
                                   .AsNoTracking()
                                   .Where(client => client.TaxpayerIndividualNumber == request.TaxpayerIndividualNumber
                                                 && client.AccountId == null)
                                   .SingleOrDefault();

            if (client == null)
            {
                return BadRequest();
            }

            var role = await _roleManager.FindOrCreateAsync(UserRole.Client);
            await _userManager.AddToRoleAsync(user, role.Name);

            client.Account = user;
            _dbContext.Update(client);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}

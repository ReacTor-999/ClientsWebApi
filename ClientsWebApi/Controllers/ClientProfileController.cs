using ClientsWebApi.Entities;
using ClientsWebApi.Models.ClientProfile;
using ClientsWebApi.Models.Clients;
using ClientsWebApi.Models.ContactInfo;
using ClientsWebApi.Models.Contracts;
using ClientsWebApi.Models.Dto;
using ClientsWebApi.Models.Founders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientsWebApi.Controllers
{
    [ApiController]
    [Route("api/client/profile")]
    [Authorize(Roles = "Client")]
    public class ClientProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientProfileController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        #region common-features

        [HttpGet]
        [Produces(typeof(ClientGetDto))]
        public async Task<IActionResult> GetDetails()
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
            {
                return NotFound();
            }

            var details = new ClientGetDto(client, client.Founders);
            return Ok(details);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(ClientProfileUpdateDto clientUpdate)
        {
            var client = await GetCurrentClientAsync();
            if(client == null)
            {
                return BadRequest("The user is not recognized as Client.");
            }

            client.Name = clientUpdate.Name;

            var entry = _dbContext.Clients.Attach(client);
            entry.Property(nameof(client.Name)).IsModified = true;

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        #endregion


        #region contact-info

        [HttpGet("contact-info")]
        [Produces(typeof(IEnumerable<ContactInfoCreateDto>))]
        public async Task<IActionResult> ListContactInfo()
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var contactInfo = _dbContext.ContactInfo
                                        .AsNoTracking()
                                        .Where(contactInfo => contactInfo.ClientId == client.Id)
                                        .Select(contactInfo => new ContactInfoGetDto(contactInfo));

            return Ok(contactInfo);
        }


        [HttpPost("contact-info")]
        public async Task<IActionResult> AddContactInfoLine([FromForm] ContactInfoCreateDto contactInfo)
        {
            var client = await GetCurrentClientAsync();
            if(client == null)
            {
                return BadRequest("The user is not recognized as Client.");
            }

            var contactInfoLine = new ContactInfo()
            {
                Email = contactInfo.Email,
                PhoneNumber = contactInfo.PhoneNumber,
                Description = contactInfo.Description,
                Other = contactInfo.Other,
                ClientId = client.Id
            };

            _dbContext.Add(contactInfoLine);

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpDelete("contact-info/{id:int}")]
        public async Task<IActionResult> DeleteContactInfoLine(int id)
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var contactInfo = new ContactInfo()
            {
                Id = id,
                ClientId = client.Id,
            };

            var entry = _dbContext.ContactInfo.Attach(contactInfo);
            entry.State = EntityState.Deleted;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpPut("contact-info/{id:int}")]
        public async Task<IActionResult> UpdateContactInfoLine(int id, [FromForm] ContactInfoUpdateDto newContactInfo)
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var contactInfo = _dbContext.ContactInfo
                                        .AsNoTracking()
                                        .Where(contactInfo => contactInfo.ClientId == client.Id
                                                           && contactInfo.Id == id)
                                        .SingleOrDefault();

            if (contactInfo == null)
            {
                return BadRequest();
            }

            var entry = _dbContext.ContactInfo.Attach(contactInfo);

            contactInfo.Email = newContactInfo.Email;
            contactInfo.PhoneNumber = newContactInfo.PhoneNumber;
            contactInfo.Description = newContactInfo.Description ?? contactInfo.Description;
            contactInfo.Other = newContactInfo.Other;

            foreach(var property in entry.Properties)
            {
                if(property.Metadata.IsKey() == false && property.CurrentValue != null)
                {
                    property.IsModified = true;
                }
            }

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        #endregion


        #region contracts

        [HttpGet("contracts")]
        [Produces(typeof(IEnumerable<ContractGetDto>))]
        public async Task<IActionResult> ListContracts()
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var contracts = _dbContext.Contracts
                                      .AsNoTracking()
                                      .Where(contract => contract.ClientId == client.Id)
                                      .Select(contract => new ContractGetDto(contract));

            return Ok(contracts);
        }

        [HttpGet("contracts/{id:int}")]
        public async Task<IActionResult> ExposeContract(int id)
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var conrtact = _dbContext.Contracts
                                     .AsNoTracking()
                                     .Where(conrtact => conrtact.Id == id
                                                     && conrtact.ClientId == client.Id)
                                     .SingleOrDefault();

            if (conrtact == null)
            {
                return NotFound();
            }

            var payments = _dbContext.Payments
                                     .AsNoTracking()
                                     .Where(payment => payment.ContractId == id);

            var result = new
            {
                Contract = conrtact,
                Payments = payments.ToList()
            };

            return Ok(result);
        }

        [HttpPost("contracts/{id:int}/payments")]
        public async Task<IActionResult> Pay(int id, PaymentCreateDto payment)
        {
            var client = await GetCurrentClientAsync();

            if (client == null)
            {
                return BadRequest();
            }

            var contract = _dbContext.Contracts
                                     .AsNoTracking()
                                     .Where(conrtact => conrtact.Id == id
                                                     && conrtact.ClientId == client.Id)
                                     .SingleOrDefault();

            if (contract == null)
            {
                return BadRequest();
            }

            var entity = new Payment()
            {
                Amount = payment.Amount,
                Contract = contract!,
                PaymentTime = DateTime.UtcNow,
                Description = payment.Description,
            };

            _dbContext.Payments.Add(entity);

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        #endregion


        #region founders

        [HttpGet("founders")]
        [Produces(typeof(IEnumerable<FounderGetDto>))]
        public async Task<IActionResult> ListFounders()
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
            {
                return BadRequest();
            }

            if(client.Founders == null)
            {
                return Ok(Enumerable.Empty<FounderGetDto>());
            }

            var founders = _dbContext.Founders
                                     .AsNoTracking()
                                     .Where(founder => client.Founders.Contains(founder))
                                     .Select(founder => new FounderGetDto(founder));

            return Ok(founders);
        }

        [HttpPost("founders")]
        public async Task<IActionResult> CreateFounder(FounderCreateDto newFounder)
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
            {
                return BadRequest();
            }

            var founder = new Founder()
            {
                TaxpayerIndividualNumber = newFounder.TIN,
                FirstName = newFounder.FirstName,
                LastName = newFounder.LastName,
                Patronymic = newFounder.Patronymic,
            };

            _dbContext.Founders.Add(founder);

            var clientEntry = _dbContext.Attach(client);

            client.Founders.Add(founder);

            clientEntry.Collection("Founders").IsModified = true;

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        [HttpPut("founders/{id:int}")]
        public async Task<IActionResult> AddExistingFounder(int id)
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
            {
                return BadRequest();
            }

            var founder = new Founder
            {
                Id = id
            };

            _dbContext.Attach(founder);

            var clientEntry = _dbContext.Clients.Attach(client);

            client.Founders.Add(founder);

            clientEntry.Collection("Founders").IsModified = true;

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        [HttpDelete("founders/{id:int}")]
        public async Task<IActionResult> DeleteFounder(int id)
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
            {
                return BadRequest();
            }

            var founder = new Founder()
            {
                Id = id
            };

            var entry =  _dbContext.Founders.Attach(founder);
            entry.State = EntityState.Deleted;

            return await this.GetDbSaveResultAsync(_dbContext);
        }

        #endregion


        private async Task<Client?> GetCurrentClientAsync()
        {
            var user = await this.GetAuthorizedUserAsync(_userManager);

            var client = _dbContext.Clients
                                   .AsNoTracking()
                                   .Where(client => client.AccountId == user.Id)
                                   .Include("Founders")
                                   .SingleOrDefault();

            return client;
        }
    }
}

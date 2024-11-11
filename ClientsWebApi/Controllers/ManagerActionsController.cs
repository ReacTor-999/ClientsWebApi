using ClientsWebApi.Entities;
using ClientsWebApi.Models.Clients;
using ClientsWebApi.Models.ContactInfo;
using ClientsWebApi.Models.Contracts;
using ClientsWebApi.Models.Dto;
using ClientsWebApi.Models.Founders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClientsWebApi.Controllers
{
    [ApiController]
    [Route("/api")]
    [Authorize(Roles = "Manager, Admin")]
    [SwaggerTag("Provides some operations for managers to interact with clients")]
    public class ManagerActionsController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ManagerActionsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("clients")]
        [SwaggerOperation(
            Summary = "Allows to get all the clients",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerListClients"
        )]
        public IActionResult ListClients()
        {
            var clients = _dbContext.Clients
                                    .AsNoTracking()
                                    .Include("Founders")
                                    .Select(client => new ClientGetDto(client, client.Founders));

            return Ok(clients);
        }


        [HttpPost("clients")]
        [SwaggerOperation(
            Summary = "Allows to create a new client",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerCrearteClient"
        )]
        public async Task<IActionResult> CreateClient(ClientCreateDto newClient)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            var client = new Client()
            {
                TaxpayerIndividualNumber = newClient.TIN,
                Name = newClient.Name,
                Type = newClient.Type,
            };

            var entry = _dbContext.Attach(client);
            entry.State = EntityState.Added;

            var dbResult = await _dbContext.TrySaveChangesAsync();

            if(dbResult.IsSuccess == false)
            {
                return BadRequest(dbResult.Message);
            }

            foreach(var id in newClient.FounderIds)
            {
                var founder = new Founder() 
                { 
                    Id = id 
                };
                var fEntry = _dbContext.Attach(founder);
                fEntry.State = EntityState.Unchanged;

                client.Founders ??= new List<Founder>();
                client.Founders.Add(founder);
            }

            entry.Collection("Founders").IsModified = true;

            dbResult = await _dbContext.TrySaveChangesAsync();

            if(dbResult.IsSuccess)
            {
                transaction.Commit();
                return Ok(dbResult.Message);
            }
            else
            {
                return BadRequest(dbResult.Message);
            }
        }


        [HttpDelete("clients/{id:int}")]
        [SwaggerOperation(
            Summary = "Allows to delete a specified client",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerDeleteClient"
        )]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = new Client()
            { 
                Id = id 
            };

            var entry = _dbContext.Clients.Attach(client);
            entry.State = EntityState.Deleted;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpGet("clients/{clientId:int}/contracts")]
        [Produces(typeof(IEnumerable<ContractGetDto>))]
        [SwaggerOperation(
            Summary = "Allows to get all the specified user's contracts",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerListClientContracts"
        )]
        public IActionResult ListClientContracts(int clientId)
        {
            var contracts = _dbContext.Contracts
                                      .AsNoTracking()
                                      .Where(contract => contract.ClientId == clientId)
                                      .Select(contract => new ContractGetDto(contract));

            return Ok(contracts);
        }


        [HttpGet("clients/{clientId:int}/contracts/{contractId:int}")]
        [Produces(typeof(ContractGetDto))]
        [SwaggerOperation(
            Summary = "Allows to get a specified contract",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerGetContract"
        )]
        public IActionResult GetContract(int clientId, int contractId)
        {
            var contract = _dbContext.Contracts
                                     .AsNoTracking()
                                     .Where(contract => contract.Id == contractId
                                                     && contract.ClientId == clientId)
                                     .Select(contract => new ContractGetDto(contract))
                                     .SingleOrDefault();

            if(contract == null)
            {
                return BadRequest();
            }
            return Ok(contract);
        }


        [HttpGet("clients/{clientId:int}/contracts/{contractId:int}/payments")]
        [Produces(typeof(PaymentGetDto[]))]
        [SwaggerOperation(
            Summary = "Allows to get all the payment provided by client to a specified contract",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerListContractPayments"
        )]
        public IActionResult ListContractPayments(int clientId, int contractId)
        {
            var contract = _dbContext.Contracts
                                     .AsNoTracking()
                                     .Where(contract => contract.Id == contractId
                                                     && contract.ClientId == clientId)
                                     .SingleOrDefault();

            if (contract == null)
            {
                return BadRequest();
            }

            var payments = _dbContext.Payments
                                     .AsNoTracking()
                                     .Where(payment => payment.ContractId == contractId)
                                     .Select(payments => new PaymentGetDto(payments));

            return Ok(payments);
        }


        [HttpPost("clients/{clientId:int}/contracts")]
        [SwaggerOperation(
            Summary = "Allows to add a new contract to the specified client",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerGetContract"
        )]
        public async Task<IActionResult> CreateContract(int clientId, ContractCreateDto newContract)
        {
            var client = _dbContext.Clients
                                   .AsNoTracking()
                                   .Where(client => client.Id == clientId)
                                   .SingleOrDefault();

            if(client == null)
            {
                return BadRequest();
            }

            var contract = new Contract()
            {
                ClientId = clientId,
                ClientTIN = client.TaxpayerIndividualNumber,
                ConclusionDate = newContract.ConclusionDate,
                ExpirationDate = newContract.ExpirationDate,
                Amount = newContract.Amount,
                IsProvided = newContract.IsProvided,
                IsPaid = newContract.IsPaid,
            };

            var entry = _dbContext.Attach(contract);
            entry.State = EntityState.Added;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpPut("clients/{clientId:int}/contracts/{contractId:int}")]
        [SwaggerOperation(
            Summary = "Allows to update the specified contract",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerUpdateContract"
        )]
        public async Task<IActionResult> UpdateContract(int clientId, int contractId, ContractUpdateDto newContract)
        {
            var client = _dbContext.Clients
                                   .AsNoTracking()
                                   .Where(client => client.Id == clientId)
                                   .SingleOrDefault();

            if (client == null)
            {
                return BadRequest();
            }

            var contract = new Contract()
            {
                Id = contractId,
                ClientId = clientId,
                ConclusionDate = newContract.ConclusionDate,
                ExpirationDate = newContract.ExpirationDate,
                Amount = newContract.Amount,
                IsProvided = newContract.IsProvided,
                IsPaid = newContract.IsPaid,
            };

            var entry = _dbContext.Attach(contract);
            
            foreach(var property in entry.Properties)
            {
                if(property.Metadata.IsKey() == false && property.CurrentValue != null)
                {
                    property.IsModified = true;
                }
            }

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpDelete("clients/{clientId:int}/contracts/{contractId:int}")]
        [SwaggerOperation(
            Summary = "Allows to delete the specified contract",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerDeleteContract"
        )]
        public async Task<IActionResult> DeleteContract(int clientId, int contractId)
        {
            var contract = _dbContext.Contracts
                                     .AsNoTracking()
                                     .Where(contract => contract.Id == contractId
                                                     && contract.ClientId == clientId)
                                     .SingleOrDefault();

            if (contract == null)
            {
                return BadRequest();
            }

            var entry = _dbContext.Contracts.Attach(contract);
            entry.State = EntityState.Deleted;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpGet("clients/{clientId:int}/contact-info")]
        [Produces(typeof(ContactInfoGetDto[]))]
        [SwaggerOperation(
            Summary = "Allows to get the specified client's contact info",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerGetContactInfo"
        )]
        public IActionResult GetContactInfo(int clientId)
        {
            var contactInfo = _dbContext.ContactInfo
                                        .AsNoTracking()
                                        .Where(contactInfo => contactInfo.ClientId == clientId)
                                        .Select(contactinfo => new ContactInfoGetDto(contactinfo));

            return Ok(contactInfo);
        }


        [HttpGet("founders")]
        [Produces(typeof(FounderGetDto[]))]
        [SwaggerOperation(
            Summary = "Allows to create a new founder",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerListFounders"
        )]
        public IActionResult ListFounders()
        {
            var founders = _dbContext.Founders
                                     .AsNoTracking()
                                     .Select(founder => new FounderGetDto(founder));

            return Ok(founders);
        }


        [HttpPost("founders")]
        [SwaggerOperation(
            Summary = "Allows to create a new founder",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerAddFounder"
        )]
        public async Task<IActionResult> AddFounder(FounderCreateDto newFounder)
        {
            var founder = new Founder()
            {
                TaxpayerIndividualNumber = newFounder.TIN,
                FirstName = newFounder.FirstName,
                LastName = newFounder.LastName,
                Patronymic = newFounder.Patronymic,
            };

            var entry = _dbContext.Founders.Attach(founder);
            entry.State = EntityState.Modified;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpDelete("founders/{id:int}")]
        [SwaggerOperation(
            Summary = "Allows to delete a specified founder",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerDeleteFounder"
        )]
        public async Task<IActionResult> DeleteFounder(int id)
        {
            var founder = new Founder()
            {
                Id = id
            };

            var entry = _dbContext.Founders.Attach(founder);
            entry.State = EntityState.Deleted;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpPut("founders/{id:int}")]
        [SwaggerOperation(
            Summary = "Allows to update a specified founder",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerUpdateFounder"
        )]
        public async Task<IActionResult> UpdateFounder(int id, FounderUpdateDto newFounder)
        {
            var founder = new Founder()
            {
                Id = id,
                FirstName = newFounder.FirstName,
                LastName = newFounder.LastName,
                Patronymic = newFounder.Patronymic,
            };

            var entry = _dbContext.Founders.Attach(founder);

            entry.Property(nameof(founder.FirstName)).IsModified = true;
            entry.Property(nameof(founder.LastName)).IsModified = true;
            entry.Property(nameof(founder.Patronymic)).IsModified = true;

            return await this.GetDbSaveResultAsync(_dbContext);
        }


        [HttpGet("contracts")]
        [Produces(typeof(IEnumerable<ContractGetDto>))]
        [SwaggerOperation(
            Summary = "Allows to get all the contracts",
            Description = "Requires manager or admin privileges",
            OperationId = "ManagerListAllContracts"
        )]
        public IActionResult ListAllContracts()
        {
            var contracts = _dbContext.Contracts
                                      .AsNoTracking()
                                      .Select(contract => new ContractGetDto(contract));

            return Ok(contracts);
        }
    }
}

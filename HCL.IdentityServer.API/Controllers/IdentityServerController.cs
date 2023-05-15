using Google.Protobuf.WellKnownTypes;
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.DTO.Builders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HCL.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IAccountService _accountService;
        private readonly ILogger<IdentityServerController> _logger;

        public IdentityServerController(IRegistrationService registrationService, IAccountService accountService
            , ILogger<IdentityServerController> logger)
        {
            _registrationService = registrationService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("v1/authenticate/")]
        public async Task<IActionResult> Authenticate([FromQuery] AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {

                return BadRequest();
            }
            var resourse = await _registrationService.Authenticate(accountDTO);
            if (resourse.StatusCode == Domain.Enums.StatusCode.AccountAuthenticate)
            {
                var log = new LogDTOBuidlder("Authenticate(accountDTO)")
                .BuildMessage($"authenticate account")
                .BuildSuccessState(resourse.Data != null)
                .BuildStatusCode(200)
                .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return Ok(resourse.Data);
            }

            return Unauthorized();
        }

        [HttpPost("v1/registration/")]
        public async Task<IActionResult> Registration([FromQuery] AccountDTO accountDTO)
        {
            var resourse = await _registrationService.Registration(accountDTO);
            if (resourse.StatusCode == Domain.Enums.StatusCode.AccountCreate)
            {
                var log = new LogDTOBuidlder("Registration(accountDTO)")
                .BuildMessage($"registration account")
                .BuildSuccessState(resourse.Data != null)
                .BuildStatusCode(201)
                .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return Created("", resourse.Data);
            }
            if (resourse.StatusCode == Domain.Enums.StatusCode.AccountExist)
            {
                var log = new LogDTOBuidlder("Registration(accountDTO)")
                .BuildMessage($"try registration account with exist data")
                .BuildStatusCode(409)
                .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return Conflict("Account Exist");
            }

            return BadRequest();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/account/")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var resourse = await _accountService.DeleteAccount(x => x.Id == id);
            if (resourse.Data)
            {
                var log = new LogDTOBuidlder("Delete(id)")
                .BuildMessage("admin account delete account")
                .BuildSuccessState(resourse.Data)
                .BuildStatusCode(204)
                .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return NoContent();
            }
            else
            {

                return BadRequest();
            }
        }
    }
}
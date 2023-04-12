using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HCL.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly ILogger<IdentityServerController> _logger;
        private readonly IRegistrationService _registrationService;
        private readonly IAccountService _accountService;

        public IdentityServerController(ILogger<IdentityServerController> logger, IRegistrationService registrationService, IAccountService accountService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _accountService = accountService;
        }

        [HttpPost("v1/Authenticate/")]
        public async Task<IActionResult> Authenticate([FromQuery] AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return BadRequest();
            }
            var resourse = await _registrationService.Authenticate(accountDTO);
            if (resourse.StatusCode == Domain.Enums.StatusCode.AccountAuthenticate)
            {
                return Ok(Results.Json(resourse.Data));
            }
            return Unauthorized();
        }

        [HttpPost("v1/Registration/")]
        public async Task<IActionResult> Registration([FromQuery] AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return BadRequest();
            }
            var resourse = await _registrationService.Registration(accountDTO);
            if (resourse.StatusCode==Domain.Enums.StatusCode.AccountCreate)
            {
                return Created("",Results.Json(resourse.Data));
            }
            return StatusCode(500);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/Account/")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var resourse = await _accountService.DeleteAccount(x => x.Id == id);
            if (resourse.Data)
            {
                return Ok(resourse.Data);
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCL.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityServerController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IAccountService _accountService;

        public IdentityServerController(IRegistrationService registrationService, IAccountService accountService)
        {
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

                return Ok(resourse.Data);
            }

            return Unauthorized();
        }

        [HttpPost("v1/Registration/")]
        public async Task<IActionResult> Registration([FromQuery] AccountDTO accountDTO)
        {
            if (ModelState.IsValid)
            {
                if (accountDTO == null)
                {

                    return BadRequest();
                }
                var resourse = await _registrationService.Registration(accountDTO);
                if (resourse.StatusCode == Domain.Enums.StatusCode.AccountCreate)
                {

                    return Created("", resourse.Data);
                }
                if (resourse.StatusCode == Domain.Enums.StatusCode.AccountExist)
                {

                    return Conflict("Account Exist");
                }
            }

            return BadRequest();
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

                return BadRequest();
            }
        }
    }
}
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCL.IdentityServer.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
        public async Task<IResult> Authenticate(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NotFound();
            }
            var resourse = await _registrationService.Authenticate(accountDTO);
            if (resourse.Data.Item1 == null)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Json(new { token = resourse.Data.Item1, id = resourse.Data.Item2 });
            }
        }

        [HttpPost("v1/Registration/")]
        public async Task<IResult> Registration(AccountDTO accountDTO)
        {
            if (accountDTO == null)
            {
                return Results.NotFound();
            }
            var resourse = await _registrationService.Registration(accountDTO);
            if (resourse.Data.Item1 == null)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Json(new { token = resourse.Data.Item1, id = resourse.Data.Item2 });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/Delete/{id}")]
        public async Task<IResult> Delete(Guid id)
        {
            var resourse = await _accountService.DeleteAccount(x => x.Id == id);
            if (resourse.Data)
            {
                return Results.Ok(resourse.Data);
            }
            else
            {
                return Results.StatusCode(500);
            }
        }
    }
}
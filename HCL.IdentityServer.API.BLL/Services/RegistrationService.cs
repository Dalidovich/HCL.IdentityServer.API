using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.DTO.Builders;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(IAccountService accountService, ITokenService tokenService
            , ILogger<RegistrationService> logger)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<BaseResponse<AuthDTO>> Registration(AccountDTO DTO)
        {
            var log = new LogDTOBuidlder("Registration(DTO)");
            var accountOnRegistration = (await _accountService.GetAccount(x => x.Login == DTO.Login)).Data;
            if (accountOnRegistration != null)
            {
                log.BuildMessage("Account with that login alredy exist");
                _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

                return new StandartResponse<AuthDTO>()
                {
                    Message = "Account with that login alredy exist",
                    StatusCode = StatusCode.AccountExist
                };
            }

            _tokenService.CreatePasswordHash(DTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newAccount = new Account(DTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));
            newAccount = (await _accountService.CreateAccount(newAccount)).Data;

            log.BuildMessage("registration account");
            _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

            return new StandartResponse<AuthDTO>()
            {
                Data = (await Authenticate(DTO)).Data,
                StatusCode = StatusCode.AccountCreate
            };
        }

        public async Task<BaseResponse<AuthDTO>> Authenticate(AccountDTO DTO)
        {
            var account = await _accountService.GetAccount(x => x.Login == DTO.Login);
            if (account.Data == null ||
                !_tokenService.VerifyPasswordHash(DTO.Password, Convert.FromBase64String(account.Data.Password), Convert.FromBase64String(account.Data.Salt)))
            {
                throw new KeyNotFoundException("[Authenticate]");
            }
            string token = _tokenService.GetToken(account.Data);

            var log = new LogDTOBuidlder("Authenticate(DTO)")
                .BuildMessage("authenticate account")
                .BuildSuccessState(true)
                .Build();
            _logger.LogInformation(JsonSerializer.Serialize(log));

            return new StandartResponse<AuthDTO>()
            {
                Data = new AuthDTO(token, (Guid)account.Data.Id),
                StatusCode = StatusCode.AccountAuthenticate
            };
        }
    }
}
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public RegistrationService(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }
        public async Task<BaseResponse<AuthDTO>> Registration(AccountDTO DTO)
        {
            var accountOnRegistration = (await _accountService.GetAccount(x => x.Login == DTO.Login)).Data;
            if (accountOnRegistration != null)
            {
                return new StandartResponse<AuthDTO>()
                {
                    Message = "Account with that login alredy exist",
                    StatusCode = StatusCode.AccountExist
                };
            }

            _tokenService.CreatePasswordHash(DTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newAccount = new Account(DTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));
            newAccount = (await _accountService.CreateAccount(newAccount)).Data;

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

            return new StandartResponse<AuthDTO>()
            {
                Data = new AuthDTO(token, (Guid)account.Data.Id),
                StatusCode = StatusCode.AccountAuthenticate
            };
        }
    }
}
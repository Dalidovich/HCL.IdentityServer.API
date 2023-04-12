using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.InnerResponse;

namespace HCL.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<AuthDTO>> Registration(AccountDTO DTO);
        public Task<BaseResponse<AuthDTO>> Authenticate(AccountDTO DTO);
        public string GetToken(Account account);
    }
}

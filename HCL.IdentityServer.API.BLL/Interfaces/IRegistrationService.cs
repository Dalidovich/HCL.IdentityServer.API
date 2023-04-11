using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.InnerResponse;

namespace HCL.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<(string, Guid)>> Registration(AccountDTO DTO);
        public Task<BaseResponse<(string, Guid)>> Authenticate(AccountDTO DTO);
        public string GetToken(Account account);
    }
}

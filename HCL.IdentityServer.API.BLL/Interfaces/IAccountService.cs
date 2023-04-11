using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.InnerResponse;
using System.Linq.Expressions;

namespace HCL.IdentityServer.API.BLL.Interfaces
{
    public interface IAccountService
    {
        public Task<BaseResponse<IEnumerable<Account>>> GetAllAccounts();
        public Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<Account>> CreateAccount(Account account);
        public Task<BaseResponse<Account>> UpdateAccount(Account account);
        public Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression);

    }
}

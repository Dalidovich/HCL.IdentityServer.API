using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository repository)
        {
            _accountRepository = repository;
        }

        public async Task<BaseResponse<Account>> CreateAccount(Account account)
        {
            var createdAccount = await _accountRepository.AddAsync(account);
            await _accountRepository.SaveAsync();

            return new StandartResponse<Account>()
            {
                Data = createdAccount,
                StatusCode = StatusCode.AccountCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {

                return new StandartResponse<bool>()
                {
                    Data = false,
                };
            }

            var accountIsDelete = _accountRepository.Delete(entity);
            await _accountRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = accountIsDelete,
                StatusCode = StatusCode.AccountDelete
            };
        }

        public async Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {

                return new StandartResponse<Account>();
            }

            return new StandartResponse<Account>()
            {
                Data = entity,
                StatusCode = StatusCode.AccountRead
            };
        }

        public async Task<BaseResponse<IEnumerable<Account>>> GetAllAccounts()
        {
            var contents = await _accountRepository.GetAll().ToListAsync();
            if (contents == null)
            {
                throw new KeyNotFoundException("[GetAllAccounts]");
            }

            return new StandartResponse<IEnumerable<Account>>()
            {
                Data = contents,
                StatusCode = StatusCode.AccountRead
            };
        }

        public async Task<BaseResponse<Account>> UpdateAccount(Account account)
        {
            var updatedAccount = _accountRepository.Update(account);
            if (updatedAccount == null)
            {
                return new StandartResponse<Account>();
            }
            await _accountRepository.SaveAsync();

            return new StandartResponse<Account>()
            {
                Data = updatedAccount,
                StatusCode = StatusCode.AccountUpdate,
            };
        }
    }
}
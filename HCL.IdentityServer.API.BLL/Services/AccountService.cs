using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        protected readonly ILogger<IAccountService> _logger;
        public AccountService(IAccountRepository repository, ILogger<IAccountService> logger)
        {
            _accountRepository = repository;
            _logger = logger;
        }
        public async Task<BaseResponse<Account>> CreateAccount(Account account)
        {
            try
            {
                var createdAccount = await _accountRepository.AddAsync(account);
                await _accountRepository.SaveAsync();

                return new StandartResponse<Account>()
                {
                    Data = createdAccount,
                    StatusCode = StatusCode.AccountCreate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CreateAccount] : {ex.Message}");

                return new StandartResponse<Account>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression)
        {
            try
            {
                var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
                if (entity == null)
                {
                    return new StandartResponse<bool>()
                    {
                        Message = "entity not found"
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[DeleteAccount] : {ex.Message}");

                return new StandartResponse<bool>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression)
        {
            try
            {
                var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
                if (entity == null)
                {
                    return new StandartResponse<Account>()
                    {
                        Message = "entity not found"
                    };
                }

                return new StandartResponse<Account>()
                {
                    Data = entity,
                    StatusCode = StatusCode.AccountRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAccount] : {ex.Message}");

                return new StandartResponse<Account>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Account>>> GetAllAccounts()
        {
            try
            {
                var contents = await _accountRepository.GetAll().ToListAsync();
                if (contents == null)
                {
                    return new StandartResponse<IEnumerable<Account>>()
                    {
                        Message = "entity not found"
                    };
                }

                return new StandartResponse<IEnumerable<Account>>()
                {
                    Data = contents,
                    StatusCode = StatusCode.AccountRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAllAccounts] : {ex.Message}");

                return new StandartResponse<IEnumerable<Account>>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Account>> UpdateAccount(Account account)
        {
            try
            {
                var updatedAccount = _accountRepository.Update(account);
                await _accountRepository.SaveAsync();

                return new StandartResponse<Account>()
                {
                    Data = updatedAccount,
                    StatusCode = StatusCode.AccountUpdate,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UpdateAccount] : {ex.Message}");

                return new StandartResponse<Account>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}

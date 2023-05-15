using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.DTO.Builders;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository repository, ILogger<AccountService> logger)
        {
            _accountRepository = repository;
            _logger = logger;
        }

        public async Task<BaseResponse<Account>> CreateAccount(Account account)
        {
            var createdAccount = await _accountRepository.AddAsync(account);
            await _accountRepository.SaveAsync();

            var log = new LogDTOBuidlder("CreateAccount(account)")
                .BuildMessage("create account")
                .BuildSuccessState(true)
                .Build();
            _logger.LogInformation(JsonSerializer.Serialize(log));

            return new StandartResponse<Account>()
            {
                Data = createdAccount,
                StatusCode = StatusCode.AccountCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression)
        {
            var log = new LogDTOBuidlder("DeleteAccount(expression)");
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                log.BuildMessage("no account");
                _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

                return new StandartResponse<bool>()
                {
                    Data = false,
                };
            }

            var accountIsDelete = _accountRepository.Delete(entity);
            await _accountRepository.SaveAsync();

            log.BuildMessage("delete account");
            _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

            return new StandartResponse<bool>()
            {
                Data = accountIsDelete,
                StatusCode = StatusCode.AccountDelete
            };
        }

        public async Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression)
        {
            var log = new LogDTOBuidlder("GetAccount(expression)");
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                log.BuildMessage("no account");
                _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

                return new StandartResponse<Account>();
            }
            log.BuildMessage("get account");
            _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

            return new StandartResponse<Account>()
            {
                Data = entity,
                StatusCode = StatusCode.AccountRead
            };
        }

        public async Task<BaseResponse<IEnumerable<Account>>> GetAccounts(Expression<Func<Account, bool>> expression)
        {
            var entities = await _accountRepository.GetAll().Where(expression).ToListAsync();

            return new StandartResponse<IEnumerable<Account>>()
            {
                Data = entities,
                StatusCode = StatusCode.AccountRead
            };
        }

        public async Task<BaseResponse<Account>> UpdateAccount(Account account)
        {
            var log = new LogDTOBuidlder("UpdateAccount(account)");
            var updatedAccount = _accountRepository.Update(account);
            if (updatedAccount == null)
            {
                log.BuildMessage("no account");
                _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

                return new StandartResponse<Account>();
            }
            await _accountRepository.SaveAsync();

            log.BuildMessage("update account");
            _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

            return new StandartResponse<Account>()
            {
                Data = updatedAccount,
                StatusCode = StatusCode.AccountUpdate,
            };
        }
    }
}
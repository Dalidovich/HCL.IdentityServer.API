using HCL.IdentityServer.API.Domain.Entities;

namespace HCL.IdentityServer.API.DAL.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<Account> AddAsync(Account entity);
        public Account Update(Account entity);
        public bool Delete(Account entity);
        public IQueryable<Account> GetAll();
        public Task<bool> SaveAsync();
    }
}

using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;

namespace HCL.IdentityServer.API.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDBContext _db;

        public AccountRepository(AppDBContext db)
        {
            _db = db;
        }

        public async Task<Account> AddAsync(Account entity)
        {
            var createdEntity = await _db.Accounts.AddAsync(entity);

            return createdEntity.Entity;
        }

        public bool Delete(Account entity)
        {
            _db.Accounts.Remove(entity);

            return true;
        }

        public IQueryable<Account> GetAll()
        {

            return _db.Accounts;
        }

        public async Task<bool> SaveAsync()
        {
            await _db.SaveChangesAsync();

            return true;
        }

        public Account Update(Account entity)
        {
            var updatedEntity = _db.Accounts.Update(entity);

            return updatedEntity.Entity;
        }
    }
}

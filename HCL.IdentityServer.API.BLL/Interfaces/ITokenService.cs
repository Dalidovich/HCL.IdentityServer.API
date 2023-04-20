using HCL.IdentityServer.API.Domain.Entities;

namespace HCL.IdentityServer.API.BLL.Interfaces
{
    public interface ITokenService
    {
        public string GetToken(Account account);
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string Password, byte[] passwordHash, byte[] passwordSalt);
    }
}

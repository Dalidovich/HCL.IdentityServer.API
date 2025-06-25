using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Enums;

namespace HCL.IdentityServer.API.Domain.Entities
{
    public class Account
    {
        public Guid? Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime CreateDate { get; set; }
        public StatusAccount StatusAccount { get; set; }
        public Role Role { get; set; }

        public Account(AccountDTO model, string salt, string password)
        {
            Login = model.Login;
            Password = password;
            CreateDate = DateTime.Now;
            StatusAccount = StatusAccount.normal;
            Salt = salt;
            Role = Role.standart;
        }

        public Account()
        {
        }
    }
}
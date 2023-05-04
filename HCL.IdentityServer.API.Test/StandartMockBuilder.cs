using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
using HCL.IdentityServer.API.Domain.JWT;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

namespace HCL.IdentityServer.API.Test
{
    public class StandartMockBuilder
    {
        public static IOptions<JWTSettings> _jwtOpt = Options.Create(new JWTSettings()
        {
            SecretKey = "jwts-learning-kjdfkjbfjbfj32242353jkdbgfkmfgk5445tjk5445ggfpogbg",
            Audience = "MyApiToken",
            Issuer = "MyClient",
        });

        private static Account _addAccount(Account account, List<Account> accounts)
        {
            var acc = new Account()
            {
                Login = account.Login,
                StatusAccount = account.StatusAccount,
                Salt = account.Salt,
                CreateDate = account.CreateDate,
                Id = Guid.NewGuid(),
                Password = account.Password,
                Role = account.Role,
            };
            accounts.Add(acc);

            return acc;
        }

        public static Mock<IAccountService> createAccountServiceMock(List<Account> accounts)
        {
            var mockAccServ = new Mock<IAccountService>();
            mockAccServ
                .Setup(s => s.GetAccount(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync((Expression<Func<Account, bool>> expression) => new StandartResponse<Account>()
                {
                    Data = accounts.Where(expression.Compile()).SingleOrDefault(),
                    StatusCode = StatusCode.AccountRead
                });

            mockAccServ
                .Setup(s => s.CreateAccount(It.IsAny<Account>()))
                .ReturnsAsync((Account account) => new StandartResponse<Account>()
                {
                    Data = _addAccount(account, accounts),
                    StatusCode = StatusCode.AccountRead
                });

            return mockAccServ;
        }
    }
}

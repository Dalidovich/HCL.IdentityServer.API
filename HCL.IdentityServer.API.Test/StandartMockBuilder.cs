using Grpc.Core;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.JWT;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;

namespace HCL.IdentityServer.API.Test
{
    public class StandartMockBuilder
    {
        public static readonly IOptions<JWTSettings> jwtOpt = Options.Create(new JWTSettings()
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

        public static Mock<IRedisLockService> CreateRedisLockServiceMock()
        {
            var redisLockServ = new Mock<IRedisLockService>();
            redisLockServ
                .Setup(r => r.SetString(It.IsAny<AthorPublicProfileReply>(), It.IsAny<string>()));

            return redisLockServ;
        }

        public static Mock<IAccountRepository> CreateAccountRepositoryMock(List<Account> accounts)
        {
            var accRep = new Mock<IAccountRepository>();
            var collectionQuerybleMock = accounts.BuildMock();
            accRep
                .Setup(r => r.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync((Account account) =>
                {

                    return _addAccount(account, accounts);
                });

            accRep.Setup(r => r.Update(It.IsAny<Account>()))
                .Returns((Account account) =>
                {
                    var updated = accounts.Where(x => x.Id == account.Id).SingleOrDefault();

                    if (updated != null)
                    {
                        accounts.Remove(updated);
                        accounts.Add(account);

                        return account;
                    }

                    return null;
                });

            accRep.Setup(r => r.SaveAsync());

            accRep.Setup(r => r.GetAll())
                .Returns(collectionQuerybleMock);


            accRep.Setup(r => r.Delete(It.IsAny<Account>()))
                .Returns((Account account) =>
                {
                    accounts.Remove(account);
                    return true;
                });

            return accRep;
        }

        public static Mock<ServerCallContext> CreateServerCallContextMock()
        {
            var serverCallContext = new Mock<ServerCallContext>();

            return serverCallContext;
        }
    }
}

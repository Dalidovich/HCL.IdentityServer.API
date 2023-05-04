using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Xunit;

namespace HCL.IdentityServer.API.Test.Services
{
    public class RegistrationServiceTest
    {
        [Fact]
        public async Task RegistrationNewAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            var authDTO = await regServ.Registration(accountForRegistration);

            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Login, accountForRegistration.Login);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.Data.accountId);
            Assert.Equal(StatusCode.AccountCreate, authDTO.StatusCode);
        }

        [Fact]
        public async Task RegistrationAlredyExistAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Registration(accountForRegistration);

            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(StatusCode.AccountExist, authDTO.StatusCode);
        }


        [Fact]
        public async Task SuccessAuntificationAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Authenticate(accountForRegistration);

            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(StatusCode.AccountAuthenticate, authDTO.StatusCode);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.Data.accountId);
        }

        [Fact]
        public async Task FailAuntificationAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await regServ.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima";

            try
            {
                await regServ.Authenticate(accountForRegistration);
            }
            catch (KeyNotFoundException ex)
            {
                Assert.NotEmpty(accounts);
                Assert.Single(accounts);
            }
            catch (Exception ex)
            {
                Assert.Fail("");
            }
        }
    }
}

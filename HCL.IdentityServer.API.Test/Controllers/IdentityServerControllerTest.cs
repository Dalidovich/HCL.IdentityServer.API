using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Controllers;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HCL.IdentityServer.API.Test.Controllers
{
    public class IdentityServerControllerTest
    {
        [Fact]
        public async Task RegistrationNewAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            var createdResult = await controller.Registration(accountForRegistration) as CreatedResult;
            Assert.NotNull(createdResult);

            var authDTO = createdResult.Value as AuthDTO;

            Assert.NotEmpty(accounts);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Login, accountForRegistration.Login);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.accountId);
        }

        [Fact]
        public async Task RegistrationAlredyExistAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await controller.Registration(accountForRegistration);
            var conflictObjectResult = await controller.Registration(accountForRegistration) as ConflictObjectResult;

            Assert.NotEmpty(accounts);
            Assert.NotNull(conflictObjectResult);
            Assert.Single(accounts);
        }

        [Fact]
        public async Task SuccessAuntificationAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await controller.Registration(accountForRegistration);
            var okObjectResult = await controller.Authenticate(accountForRegistration) as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var authDTO = okObjectResult.Value as AuthDTO;

            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.accountId);
        }

        [Fact]
        public async Task FailAuntificationAccountTest()
        {
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.createAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            await controller.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima";

            try
            {
                await controller.Authenticate(accountForRegistration);
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

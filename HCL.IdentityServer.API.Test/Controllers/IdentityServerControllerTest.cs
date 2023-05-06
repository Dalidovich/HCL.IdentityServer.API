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
        public async Task Registration_WithRightAuthData_ReturnCreatedResult()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            var createdResult = await controller.Registration(accountForRegistration) as CreatedResult;

            //Assert
            Assert.NotNull(createdResult);

            var authDTO = createdResult.Value as AuthDTO;

            Assert.NotEmpty(accounts);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Login, accountForRegistration.Login);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.accountId);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnConflictObjectResult()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var conflictObjectResult = await controller.Registration(accountForRegistration) as ConflictObjectResult;

            //Assert
            Assert.NotEmpty(accounts);
            Assert.NotNull(conflictObjectResult);
            Assert.Single(accounts);
        }

        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnOkObjectResult()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var okObjectResult = await controller.Authenticate(accountForRegistration) as OkObjectResult;

            //Assert
            Assert.NotNull(okObjectResult);

            var authDTO = okObjectResult.Value as AuthDTO;

            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.accountId);
        }

        [Fact]
        public async Task AuntificationAccount_WithWrongAuthData_ReturnKeyNotFoundException()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima";

            try
            {
                await controller.Authenticate(accountForRegistration);

                //Assert
                Assert.Fail("");
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

        [Fact]
        public async Task DeleteAccount_WithExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var okObjectResult = await controller.Delete((Guid)accounts.First().Id) as OkObjectResult;

            //Assert
            Assert.NotNull(okObjectResult);
            Assert.Empty(accounts);
        }

        [Fact]
        public async Task DeleteAccount_WithNotExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);

            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            var regServ = new RegistrationService(mockAccServ.Object, tokServ);
            var controller = new IdentityServerController(regServ, mockAccServ.Object);

            //Act
            var badRequestResult = await controller.Delete(Guid.NewGuid()) as BadRequestResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Empty(accounts);
        }
    }
}

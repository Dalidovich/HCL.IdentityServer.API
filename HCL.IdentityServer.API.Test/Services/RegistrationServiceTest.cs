using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Controllers;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Xunit;

namespace HCL.IdentityServer.API.Test.Services
{
    public class RegistrationServiceTest
    {
        [Fact]
        public async Task Registration_WithRightAuthData_ReturnNewAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            var authDTO = await regServ.Registration(accountForRegistration);

            //Assert
            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Login, accountForRegistration.Login);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.Data.accountId);
            Assert.Equal(StatusCode.AccountCreate, authDTO.StatusCode);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnExistStatusCode()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Registration(accountForRegistration);

            //Assert
            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(StatusCode.AccountExist, authDTO.StatusCode);
        }


        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnAuthenticateStatusCode()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Authenticate(accountForRegistration);

            //Assert
            Assert.NotEmpty(accounts);
            Assert.NotNull(authDTO);
            Assert.Equal(StatusCode.AccountAuthenticate, authDTO.StatusCode);
            Assert.Equal(accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id, authDTO.Data.accountId);
        }

        [Fact]
        public async Task AuntificationAccount_WithWrongAuthData_ReturnKeyNotFoundException()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima";

            try
            {
                await regServ.Authenticate(accountForRegistration);

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
    }
}

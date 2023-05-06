using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Xunit;

namespace HCL.IdentityServer.API.Test.Services
{
    public class AccountServiceTest
    {
        [Fact]
        public async Task AddAccount_WithRightData_ReturnNewAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            var newAccount = new Account()
            {
                Login = "Ilia",
                Salt = "salt",
                Password = "password",
                StatusAccount = StatusAccount.normal,
                Role = Role.standart,
                CreateDate = DateTime.Now,
            };

            //Act
            var addedAccount = await accountServ.CreateAccount(newAccount);

            //Assert
            Assert.NotEmpty(accounts);
            Assert.Equal(addedAccount.Data.Login, newAccount.Login);
            Assert.Equal(addedAccount.Data.Login, accounts.First().Login);
            Assert.Equal(addedAccount.StatusCode, StatusCode.AccountCreate);
        }

        [Fact]
        public async Task UpdateAccount_WithExistAccount_ReturnUpdatedAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>()
            {
                new Account()
                {
                    Id=Guid.NewGuid(),
                    Login = "Ilia",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            var newAccount = new Account()
            {
                Id = accounts.First().Id,
                Login = accounts.First().Login,
                Salt = accounts.First().Salt,
                Password = accounts.First().Password,
                StatusAccount = accounts.First().StatusAccount,
                Role = accounts.First().Role,
                CreateDate = accounts.First().CreateDate,
            };
            newAccount.Login = "Dima";

            //Act
            var addedAccount = await accountServ.UpdateAccount(newAccount);

            //Assert
            Assert.NotEmpty(accounts);
            Assert.Equal(newAccount.Login, accounts.First().Login);
            Assert.Equal(addedAccount.Data.Login, accounts.First().Login);
            Assert.Equal(addedAccount.StatusCode, StatusCode.AccountUpdate);
        }

        [Fact]
        public async Task UpdateAccount_WithNotExistAccount_ReturnUpdatedAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            var newAccount = new Account()
            {
                Id = Guid.NewGuid(),
                Login = "Dima"
            };

            //Act
            var addedAccount = await accountServ.UpdateAccount(newAccount);

            //Assert
            Assert.Empty(accounts);
            Assert.Equal(addedAccount.StatusCode, StatusCode.EntityNotFound);
        }
    }
}

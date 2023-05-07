using FluentAssertions;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.InnerResponse;
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
            accounts.Should().NotBeEmpty();
            addedAccount.Data.Login.Should().Be(newAccount.Login).And.Be(accounts.First().Login);
            addedAccount.StatusCode.Should().Be(StatusCode.AccountCreate);
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
            accounts.Should().NotBeEmpty();
            accounts.First().Login.Should().Be(newAccount.Login).And.Be(addedAccount.Data.Login);
            addedAccount.StatusCode.Should().Be(StatusCode.AccountUpdate);
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
            accounts.Should().BeEmpty();
            addedAccount.StatusCode.Should().Be(StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task DeleteAccount_WithExistAccount_ReturnBooleantTrue()
        {
            //Arrange
            List<Account> accounts = new List<Account>()
            {
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Dima"
                }
            };

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == "Dima");

            //Assert
            accounts.Should().BeEmpty();
            deleteConfirm.Data.Should().BeTrue();
            deleteConfirm.StatusCode.Should().Be(StatusCode.AccountDelete);
        }

        [Fact]
        public async Task DeleteAccount_WithNotExistAccount_ReturnBooleantTrue()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == "Dima");

            //Assert
            accounts.Should().BeEmpty();
            deleteConfirm.Data.Should().BeFalse();
            deleteConfirm.StatusCode.Should().Be(StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task GetAccount_WithExistAccount_ReturnAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>()
            {
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Dima"
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Ilia",
                    StatusAccount=StatusAccount.deleted
                }
            };

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var account = await accountServ.GetAccount(x=>x.Login=="Ilia");

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.AccountRead);
            account.Data.Should().NotBeNull();
            account.Data.StatusAccount.Should().Be(StatusAccount.deleted);
        }

        [Fact]
        public async Task GetAccount_WithNotExistAccount_ReturnAccount()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var account = await accountServ.GetAccount(x => x.Login == "Ilia");

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.EntityNotFound);
            account.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetAccounts_WithExistAccount_ReturnAccounts()
        {
            //Arrange
            List<Account> accounts = new List<Account>()
            {
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Dima"
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Max"
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Ilia",
                    StatusAccount=StatusAccount.deleted
                }
            };

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var account = await accountServ.GetAccounts(x => x.StatusAccount == StatusAccount.normal);

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.AccountRead);
            account.Data.Should().NotBeNull();
            account.Data.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAccounts_WithNotExistAccount_ReturnAccounts()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var accountServ = new AccountService(mockAccRep.Object);

            //Act
            var account = await accountServ.GetAccounts(x => x.Login == "Ilia");

            //Assert
            account.Should().NotBeNull();
            account.Data.Should().NotBeNull();
            account.Data.Should().BeEmpty();
        }
    }
}

using DotNet.Testcontainers.Containers;
using FluentAssertions;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.IdentityServer.API.Test.IntegrationTest.Services
{
    public class AccountServiceIntegrationTest
    {
        [Fact]
        public async Task AddAccount_WithRightData_ReturnNewAccount()
        {
            //Arrange
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

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
            addedAccount.Data.Login.Should().Be(newAccount.Login);
            addedAccount.StatusCode.Should().Be(StatusCode.AccountCreate);

            await pgContainer.DisposeAsync();
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
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAccounts(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB, accounts);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

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
            addedAccount.StatusCode.Should().Be(StatusCode.AccountUpdate);
            addedAccount.Data.Login.Should().Be(newAccount.Login);

            await pgContainer.DisposeAsync();
        }

        [Fact]
        public async Task UpdateAccount_WithNotExistAccount_ReturnError()
        {
            //Arrange
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            var newAccount = new Account()
            {
                Id = Guid.NewGuid(),
                Login = "Dima",
                Salt = "salt",
                Password = "password",
                StatusAccount = StatusAccount.normal,
                Role = Role.standart,
                CreateDate = DateTime.Now,
            };

            //Act
            var result = async () =>
            {
                var addedAccount = await accountServ.UpdateAccount(newAccount);
            };

            //Assert
            result.Should().ThrowAsync<KeyNotFoundException>();


            await pgContainer.DisposeAsync();
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
                    Login = "Dima",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };

            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAccounts(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB, accounts);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == "Dima");

            //Assert
            deleteConfirm.Data.Should().BeTrue();
            deleteConfirm.StatusCode.Should().Be(StatusCode.AccountDelete);

            await pgContainer.DisposeAsync();
        }

        [Fact]
        public async Task DeleteAccount_WithNotExistAccount_ReturnBooleantFalse()
        {
            //Arrange
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == "Dima");

            //Assert
            deleteConfirm.Data.Should().BeFalse();
            deleteConfirm.StatusCode.Should().Be(StatusCode.EntityNotFound);

            await pgContainer.DisposeAsync();
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
                    Login = "Dima",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Ilia",
                    StatusAccount=StatusAccount.deleted,
                    Salt = "salt",
                    Password = "password",
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAccounts(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB, accounts);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccount(x => x.Login == "Ilia");

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.AccountRead);
            account.Data.Should().NotBeNull();
            account.Data.StatusAccount.Should().Be(StatusAccount.deleted);

            await pgContainer.DisposeAsync();
        }

        [Fact]
        public async Task GetAccount_WithNotExistAccount_ReturnAccount()
        {
            //Arrange
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccount(x => x.Login == "Ilia");

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.EntityNotFound);
            account.Data.Should().BeNull();

            await pgContainer.DisposeAsync();
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
                    Login = "Dima",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Max",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Ilia",
                    Salt = "salt",
                    Password = "password",
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                    StatusAccount=StatusAccount.deleted
                }
            };
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAccounts(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB, accounts);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccounts(x => x.StatusAccount == StatusAccount.normal);

            //Assert
            account.Should().NotBeNull();
            account.StatusCode.Should().Be(StatusCode.AccountRead);
            account.Data.Should().NotBeNull();
            account.Data.Count().Should().Be(2);

            await pgContainer.DisposeAsync();
        }

        [Fact]
        public async Task GetAccounts_WithNotExistAccount_ReturnAccounts()
        {
            //Arrange
            IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccounts(x => x.Login == "Ilia");

            //Assert
            account.Should().NotBeNull();
            account.Data.Should().NotBeNull();
            account.Data.Should().BeEmpty();

            await pgContainer.DisposeAsync();
        }
    }
}

﻿using DotNet.Testcontainers.Containers;
using FluentAssertions;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.IdentityServer.API.Test.IntegrationTest.Services
{
    public class AccountServiceIntegrationTest : IAsyncLifetime
    {
        IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
        private WebApplicationFactory<Program> webHost;

        public async Task InitializeAsync()
        {
            await pgContainer.StartAsync();
            webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
        }

        public async Task DisposeAsync()
        {
            await pgContainer.StopAsync();
        }

        [Fact]
        public async Task AddAccount_WithRightData_ReturnNewAccount()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            var newAccount = new Account()
            {
                Login = "IliaAS1",
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
                    Login = "IliaAS2",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);

            accounts.ForEach(async x => await accRep.AddAsync(x));
            await accRep.SaveAsync();

            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            var newAccount = await accRep.GetAll().Where(a => a.Id == accounts.First().Id).SingleOrDefaultAsync();
            newAccount.Login = "DimaAS2";

            //Act
            var addedAccount = await accountServ.UpdateAccount(newAccount);

            //Assert
            addedAccount.StatusCode.Should().Be(StatusCode.AccountUpdate);
            addedAccount.Data.Login.Should().Be(newAccount.Login);
        }

        [Fact]
        public async Task UpdateAccount_WithNotExistAccount_ReturnError()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            var newAccount = new Account()
            {
                Id = Guid.NewGuid(),
                Login = "DimaAS3",
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
            await result.Should().ThrowAsync<DbUpdateConcurrencyException>();
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
                    Login = "DimaAS4",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);

            accounts.ForEach(async x => await accRep.AddAsync(x));
            await accRep.SaveAsync();

            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == accounts.First().Login);

            //Assert
            deleteConfirm.Data.Should().BeTrue();
            deleteConfirm.StatusCode.Should().Be(StatusCode.AccountDelete);
        }

        [Fact]
        public async Task DeleteAccount_WithNotExistAccount_ReturnBooleantFalse()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var deleteConfirm = await accountServ.DeleteAccount(x => x.Login == "DimaNotExist");

            //Assert
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
                    Login = "DimaAS6",
                    Salt = "salt",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "IliaAS6",
                    StatusAccount=StatusAccount.deleted,
                    Salt = "salt",
                    Password = "password",
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                }
            };
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);

            accounts.ForEach(async x => await accRep.AddAsync(x));
            await accRep.SaveAsync();

            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccount(x => x.Login == "IliaAS6");

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
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccount(x => x.Login == "IliaNotExist");

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
                    Login = "DimaAS8",
                    Salt = "salt2",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "MaxAS8",
                    Salt = "salt2",
                    Password = "password",
                    StatusAccount = StatusAccount.normal,
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                },
                new Account()
                {
                    Id = Guid.NewGuid(),
                    Login = "Ilia8",
                    Salt = "salt",
                    Password = "password",
                    Role = Role.standart,
                    CreateDate = DateTime.Now,
                    StatusAccount=StatusAccount.deleted
                }
            };
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);

            accounts.ForEach(async x => await accRep.AddAsync(x));
            await accRep.SaveAsync();

            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccounts(x => x.Salt == "salt2");

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
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);

            //Act
            var account = await accountServ.GetAccounts(x => x.Login == "IliaNotExist");

            //Assert
            account.Should().NotBeNull();
            account.Data.Should().NotBeNull();
            account.Data.Should().BeEmpty();
        }
    }
}
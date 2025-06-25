using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Test.IntegrationTest;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.IdentityServer.API.Test.IntegrationTest.Services
{
    public class AthorPublicProfileServiceIntegrationTest : IAsyncLifetime
    {
        IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
        private AthorPublicProfileService publicProfileService;
        private AccountRepository accountRepository;

        public async Task InitializeAsync()
        {
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            accountRepository = new AccountRepository(appDBContext);
            var accountService = new AccountService(accountRepository, StandartMockBuilder.mockLoggerAccServ);
            var mockRedisLockServ = StandartMockBuilder.CreateRedisLockServiceMock();
            publicProfileService = new AthorPublicProfileService(accountService, mockRedisLockServ.Object, StandartMockBuilder.mockLoggerAthorPublServ);

        }

        public async Task DisposeAsync()
        {
            await pgContainer.StopAsync();
        }

        [Fact]
        public async Task GetProfile_WithExistAccount_ReturnProfile()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account()
                {
                    Id=Guid.NewGuid(),
                    Login="Ilia1",
                    CreateDate=DateTime.MaxValue,
                    StatusAccount=StatusAccount.normal,
                    Password="password",
                    Role=Role.standart,
                    Salt="salt"
                }
            };

            accounts.ForEach(async x => await accountRepository.AddAsync(x));
            await accountRepository.SaveAsync();            

            var req = new AthorIdRequest()
            {
                AccountId = accounts.First().Id.ToString()
            };

            var expectedReply = new AthorPublicProfileReply()
            {
                Login = accounts.First().Login,
                CreateDate = Timestamp.FromDateTimeOffset(accounts.First().CreateDate),
                Status = accounts.First().StatusAccount.ToString()
            };

            //Act
            var actualReply = await publicProfileService.GetProfile(req, StandartMockBuilder.CreateServerCallContextMock().Object);

            //Assert
            accounts.Should().NotBeEmpty();
            actualReply.Should().Be(expectedReply);
        }

        [Fact]
        public async Task GetProfile_WithNotExistAccount_ReturnDefaultProfile()
        {
            //Arrange
            var req = new AthorIdRequest()
            {
                AccountId = Guid.NewGuid().ToString(),
            };

            var expectedReply = new AthorPublicProfileReply()
            {
                Login = "-",
                Status = "not exist",
                CreateDate = Timestamp.FromDateTimeOffset(DateTime.Now)
            };

            //Act
            var actualReply = await publicProfileService.GetProfile(req, StandartMockBuilder.CreateServerCallContextMock().Object);

            //Assert
            actualReply.Status.Should().Be(expectedReply.Status);
            actualReply.Login.Should().Be(expectedReply.Login);
        }
    }
}

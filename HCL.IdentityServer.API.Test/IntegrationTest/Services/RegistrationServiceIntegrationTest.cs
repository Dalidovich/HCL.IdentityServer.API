using DotNet.Testcontainers.Containers;
using FluentAssertions;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Test.IntegrationTest;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.IdentityServer.API.Test.IntegrationTest.Services
{
    public class RegistrationServiceIntegrationTest : IAsyncLifetime
    {
        IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();
        private RegistrationService registrationService;
        private AccountRepository accountRepository;

        public async Task InitializeAsync()
        {
            await pgContainer.StartAsync();
            var webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);

            var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            accountRepository = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accountRepository, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            registrationService = new RegistrationService(accountServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
        }

        public async Task DisposeAsync()
        {
            await pgContainer.StopAsync();
        }

        [Fact]
        public async Task Registration_WithRightAuthData_ReturnNewAccount()
        {
            //Arrange
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia1",
                Password = "123456",
            };

            //Act
            var authDTO = await registrationService.Registration(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.Data.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountCreate);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnExistStatusCode()
        {
            //Arrange
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia2",
                Password = "123456",
            };

            //Act
            await registrationService.Registration(accountForRegistration);
            var authDTO = await registrationService.Registration(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountExist);
        }


        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnAuthenticateStatusCode()
        {
            //Arrange
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia3",
                Password = "123456",
            };

            //Act
            await registrationService.Registration(accountForRegistration);
            var authDTO = await registrationService.Authenticate(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountAuthenticate);
        }

        [Fact]
        public async Task AuntificationAccount_WithWrongAuthData_ReturnKeyNotFoundException()
        {
            //Arrange
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia4",
                Password = "123456",
            };

            //Act
            await registrationService.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima4";

            var result = async () =>
            {
                await registrationService.Authenticate(accountForRegistration);
            };

            await result.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}

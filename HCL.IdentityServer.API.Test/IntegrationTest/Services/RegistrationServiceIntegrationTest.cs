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
        public async Task Registration_WithRightAuthData_ReturnNewAccount()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accountServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia1",
                Password = "123456",
            };

            //Act
            var authDTO = await regServ.Registration(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.Data.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountCreate);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnExistStatusCode()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accountServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia2",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Registration(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountExist);
        }


        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnAuthenticateStatusCode()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accountServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia3",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);
            var authDTO = await regServ.Authenticate(accountForRegistration);

            //Assert
            authDTO.Should().NotBeNull();
            authDTO.StatusCode.Should().Be(StatusCode.AccountAuthenticate);
        }

        [Fact]
        public async Task AuntificationAccount_WithWrongAuthData_ReturnKeyNotFoundException()
        {
            //Arrange
            using var scope = webHost.Services.CreateScope();
            var appDBContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var accRep = new AccountRepository(appDBContext);
            var accountServ = new AccountService(accRep, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accountServ, tokServ, StandartMockBuilder.mockLoggerRegServ);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia4",
                Password = "123456",
            };

            //Act
            await regServ.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima4";

            var result = async () =>
            {
                await regServ.Authenticate(accountForRegistration);
            };

            await result.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}

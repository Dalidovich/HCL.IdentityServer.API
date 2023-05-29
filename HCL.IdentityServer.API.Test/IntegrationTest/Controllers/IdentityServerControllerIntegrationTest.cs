using DotNet.Testcontainers.Containers;
using FluentAssertions;
using HCL.IdentityServer.API.Domain.DTO;
using Newtonsoft.Json;
using Xunit;

namespace HCL.IdentityServer.API.Test.IntegrationTest.Controllers
{
    public class IdentityServerControllerIntegrationTest:IAsyncLifetime
    {
        IContainer pgContainer = TestContainerBuilder.CreatePostgreSQLContainer();

        public async Task InitializeAsync()
        {
            await pgContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await pgContainer.StopAsync();
        }

        [Fact]
        public async Task Registration_WithRightAuthData_ReturnCreatedResult()
        {
            //Arrange
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia1",
                Password = "123456",
            };

            //Act
            HttpResponseMessage createdResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            //Assert
            createdResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnConflictObjectResult()
        {
            //Arrange            
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia2",
                Password = "123456",
            };

            //Act
            HttpResponseMessage createdFirstResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            HttpResponseMessage createdSecondResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);

            //Assert
            createdFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            createdSecondResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnOkObjectResult()
        {
            //Arrange            
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia3",
                Password = "123456",
            };

            //Act
            HttpResponseMessage createdFirstResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            HttpResponseMessage authenticateResult = await client.PostAsync($"api/IdentityServer/v1/authenticate" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);

            //Assert
            createdFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            authenticateResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuntificationAccount_WithWrongAuthData_ReturnKeyNotFoundException()
        {
            //Arrange            
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia4",
                Password = "123456",
            };

            //Act
            HttpResponseMessage createdFirstResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            accountForRegistration.Login = "Dima";
            HttpResponseMessage authenticateResult = await client.PostAsync($"api/IdentityServer/v1/authenticate" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);

            //Assert
            createdFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            authenticateResult.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAccount_WithoutAuth_ReturnOkTrueBoolean()
        {
            //Arrange            
            var webHost = CustomTestHostBuilder.Build(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia5",
                Password = "123456",
            };

            //Act
            HttpResponseMessage createdFirstResult = await client.PostAsync($"api/IdentityServer/v1/Registration" +
                $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            AuthDTO authDTO = JsonConvert.DeserializeObject<AuthDTO>((await createdFirstResult.Content.ReadAsStringAsync()));
            HttpResponseMessage deleteResult = await client.DeleteAsync($"api/IdentityServer/v1/account?id={authDTO.accountId}");

            //Assert
            createdFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            deleteResult.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteAccount_WithAuthAndExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange          
            var webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "admin",
                Password = "admin",
            };

            //Act
            HttpResponseMessage authenticateFirstResult = await client.PostAsync($"api/IdentityServer/v1/authenticate" +
                 $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            AuthDTO authDTO = JsonConvert.DeserializeObject<AuthDTO>((await authenticateFirstResult.Content.ReadAsStringAsync()));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authDTO.JWTToken);

            HttpResponseMessage deleteResult = await client.DeleteAsync($"api/IdentityServer/v1/account?id={authDTO.accountId}");

            //Assert
            authenticateFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deleteResult.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteAccount_WithAuthAndNotExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange          
            var webHost = CustomTestHostBuilder.BuildWithAdmin(TestContainerBuilder.npgsqlUser, TestContainerBuilder.npgsqlPassword
                , "localhost", pgContainer.GetMappedPublicPort(5432), TestContainerBuilder.npgsqlDB);
            var client = webHost.CreateClient();
            var accountForRegistration = new AccountDTO()
            {
                Login = "admin",
                Password = "admin",
            };

            //Act
            HttpResponseMessage authenticateFirstResult = await client.PostAsync($"api/IdentityServer/v1/authenticate" +
                 $"?Login={accountForRegistration.Login}&Password={accountForRegistration.Password}", null);
            AuthDTO authDTO = JsonConvert.DeserializeObject<AuthDTO>((await authenticateFirstResult.Content.ReadAsStringAsync()));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authDTO.JWTToken);

            HttpResponseMessage deleteResult = await client.DeleteAsync($"api/IdentityServer/v1/account?id={Guid.NewGuid()}");

            //Assert
            authenticateFirstResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deleteResult.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}

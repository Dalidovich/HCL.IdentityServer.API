﻿using FluentAssertions;
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

            var mockAccRep=StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

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

            accounts.Should().HaveCountGreaterThan(0);
            accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Login.Should().Be(accountForRegistration.Login);
            accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id.Should().Be(authDTO.accountId);
        }

        [Fact]
        public async Task Registration_WithAlredyExistAccount_ReturnConflictObjectResult()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var conflictObjectResult = await controller.Registration(accountForRegistration) as ConflictObjectResult;

            //Assert
            accounts.Should().HaveCountGreaterThan(0);
            conflictObjectResult.Should().NotBeNull();
            accounts.Should().ContainSingle();
        }

        [Fact]
        public async Task AuntificationAccount_WithRightAuthData_ReturnOkObjectResult()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var okObjectResult = await controller.Authenticate(accountForRegistration) as OkObjectResult;

            //Assert
            okObjectResult.Should().NotBeNull();

            var authDTO = okObjectResult.Value as AuthDTO;

            accounts.Where(x => x.Login == accountForRegistration.Login).SingleOrDefault().Id.Should().Be(authDTO.accountId);
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
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);

            accountForRegistration.Login = "Dima";

            var result = async () =>
            {
                await controller.Authenticate(accountForRegistration);
            };

            await result.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteAccount_WithExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

            var accountForRegistration = new AccountDTO()
            {
                Login = "Ilia",
                Password = "123456",
            };

            //Act
            await controller.Registration(accountForRegistration);
            var noContentResult = await controller.Delete((Guid)accounts.First().Id) as NoContentResult;

            //Assert
            noContentResult.Should().NotBeNull();
            accounts.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAccount_WithNotExistAccount_ReturnOkTrueBoolean()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);

            var accServ = new AccountService(mockAccRep.Object, StandartMockBuilder.mockLoggerAccServ);
            var tokServ = new TokenService(StandartMockBuilder.jwtOpt);
            var regServ = new RegistrationService(accServ, tokServ, StandartMockBuilder.mockLoggerRegServ);
            var controller = new IdentityServerController(regServ, accServ, StandartMockBuilder.mockLoggerController);

            //Act
            var badRequestResult = await controller.Delete(Guid.NewGuid()) as BadRequestResult;

            //Assert
            badRequestResult.Should().NotBeNull();
            accounts.Should().BeEmpty();
        }
    }
}

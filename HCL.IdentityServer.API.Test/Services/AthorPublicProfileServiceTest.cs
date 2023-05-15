using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using Xunit;

namespace HCL.IdentityServer.API.Test.Services
{
    public class AthorPublicProfileServiceTest
    {
        [Fact]
        public async Task GetProfile_WithExistAccount_ReturnProfile()
        {
            //Arrange
            List<Account> accounts = new List<Account>
            {
                new Account()
                {
                    Id=Guid.NewGuid(),
                    Login="Ilia",
                    CreateDate=DateTime.MaxValue,
                    StatusAccount=StatusAccount.normal
                }
            };

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var mockRedisLockServ = StandartMockBuilder.CreateRedisLockServiceMock();

            var accServ = new AccountService(mockAccRep.Object);
            var AthorPublicProfileServ = new AthorPublicProfileService(accServ, mockRedisLockServ.Object);

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
            var actualReply = await AthorPublicProfileServ.GetProfile(req, StandartMockBuilder.CreateServerCallContextMock().Object);

            //Assert
            accounts.Should().NotBeEmpty();
            actualReply.Should().Be(expectedReply);
        }

        [Fact]
        public async Task GetProfile_WithNotExistAccount_ReturnDefaultProfile()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccRep = StandartMockBuilder.CreateAccountRepositoryMock(accounts);
            var mockRedisLockServ = StandartMockBuilder.CreateRedisLockServiceMock();

            var accServ = new AccountService(mockAccRep.Object);
            var AthorPublicProfileServ = new AthorPublicProfileService(accServ, mockRedisLockServ.Object);

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
            var actualReply = await AthorPublicProfileServ.GetProfile(req, StandartMockBuilder.CreateServerCallContextMock().Object);

            //Assert
            accounts.Should().BeEmpty();
            actualReply.Status.Should().Be(expectedReply.Status);
            actualReply.Login.Should().Be(expectedReply.Login);
        }
    }
}

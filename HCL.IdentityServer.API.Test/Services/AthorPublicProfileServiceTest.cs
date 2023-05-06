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

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);
            var mockRedisLockServ = StandartMockBuilder.CreateRedisLockServiceMock();

            var AthorPublicProfileServ = new AthorPublicProfileService(mockAccServ.Object, mockRedisLockServ.Object);

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
            Assert.NotEmpty(accounts);
            Assert.Equal(actualReply, expectedReply);
        }

        [Fact]
        public async Task GetProfile_WithNotExistAccount_ReturnDefaultProfile()
        {
            //Arrange
            List<Account> accounts = new List<Account>();

            var mockAccServ = StandartMockBuilder.CreateAccountServiceMock(accounts);
            var mockRedisLockServ = StandartMockBuilder.CreateRedisLockServiceMock();

            var AthorPublicProfileServ = new AthorPublicProfileService(mockAccServ.Object, mockRedisLockServ.Object);

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
            Assert.Empty(accounts);
            Assert.Equal(actualReply.Status, expectedReply.Status);
            Assert.Equal(actualReply.Login, expectedReply.Login);
        }
    }
}

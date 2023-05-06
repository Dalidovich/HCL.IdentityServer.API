using HCL.IdentityServer.API.BLL.Services;
using Xunit;

namespace HCL.IdentityServer.API.Test.Services
{
    public class TokenServiceTest
    {
        [Fact]
        public void RightСonsistencyPasswordHashAlgorithm_WithRightData_ReturnRightCoincidence()
        {
            //Arrange
            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            string password = "123456";

            //Act
            tokServ.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var salt = Convert.ToBase64String(passwordSalt);
            var passwordHashStr = Convert.ToBase64String(passwordHash);

            //Assert
            Assert.True(tokServ.VerifyPasswordHash(password, Convert.FromBase64String(passwordHashStr), Convert.FromBase64String(salt)));
        }

        [Fact]
        public void RightСonsistencyPasswordHashAlgorithm_WithWrongData_ReturnRightCoincidence()
        {
            //Arrange
            var tokServ = new TokenService(StandartMockBuilder._jwtOpt);
            string password = "123456";

            //Act
            tokServ.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var salt = Convert.ToBase64String(passwordSalt);
            var passwordHashStr = Convert.ToBase64String(passwordHash);

            password = "654321";

            //Assert
            Assert.False(tokServ.VerifyPasswordHash(password, Convert.FromBase64String(passwordHashStr), Convert.FromBase64String(salt)));
        }
    }
}

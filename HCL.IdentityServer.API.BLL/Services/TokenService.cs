using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.Domain.JWT;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _options;

        public TokenService(IOptions<JWTSettings> options)
        {
            _options = options.Value;
        }

        public string GetToken(Account account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,account.Login),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim(CustomClaimType.AccountId, account.Id.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(StandartConst.StartJWTTokenLifeTime)),
                    notBefore: DateTime.Now,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string Password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}

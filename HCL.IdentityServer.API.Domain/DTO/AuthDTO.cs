using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.IdentityServer.API.Domain.DTO
{
    public record AuthDTO
    {
        public string JWTToken { get; set; }
        public Guid accountId { get; set; }

        public AuthDTO(string jWTToken, Guid accountId)
        {
            JWTToken = jWTToken;
            this.accountId = accountId;
        }
    }
}

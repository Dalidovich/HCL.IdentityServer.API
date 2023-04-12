using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.IdentityServer.API.Domain.DTO
{
    public record AuthDTO(string JWTToken,Guid accountId);
}

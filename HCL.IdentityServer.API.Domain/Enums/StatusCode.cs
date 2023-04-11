using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.IdentityServer.API.Domain.Enums
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        AccountCreate = 1,
        AccountUpdate = 2,
        AccountDelete = 3,
        AccountRead = 4,
        AccountAuthenticate = 5,

        OK = 200,
        OKNoContent = 204,
        InternalServerError = 500,
    }
}

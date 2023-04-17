using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.IdentityServer.API.Domain.DTO
{
    public class AccountDTO
    {
        [Required(AllowEmptyStrings = true,ErrorMessage = "Need Login")]
        public string Login { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Need Password")]
        public string Password { get; set; }
    }
}

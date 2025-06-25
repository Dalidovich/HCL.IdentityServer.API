using System.ComponentModel.DataAnnotations;

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
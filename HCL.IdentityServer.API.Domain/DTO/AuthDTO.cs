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
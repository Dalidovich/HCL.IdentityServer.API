using HCL.IdentityServer.API.Domain.Enums;

namespace HCL.IdentityServer.API.Domain.InnerResponse
{
    public abstract class BaseResponse<T>
    {
        public virtual T Data { get; set; }
        public virtual StatusCode StatusCode { get; set; }
        public virtual string Message { get; set; }
    }
}
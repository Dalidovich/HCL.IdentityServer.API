using HCL.IdentityServer.API.BLL.gRPCServices;

namespace HCL.IdentityServer.API.BLL.Interfaces
{
    public interface IRedisLockService
    {
        public Task SetString(AthorPublicProfileReply reply, string id);
    }
}

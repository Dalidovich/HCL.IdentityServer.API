using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AthorPublicProfileService : AthorPublicProfile.AthorPublicProfileBase
    {
        private readonly IAccountService _accountService;
        private readonly IRedisLockService _redisLockService;

        public AthorPublicProfileService(IAccountService accountService, IRedisLockService redisLockService)
        {
            _accountService = accountService;
            _redisLockService = redisLockService;
        }

        public override async Task<AthorPublicProfileReply> GetProfile(AthorIdRequest request, ServerCallContext context)
        {
            var account = await _accountService.GetAccount(x => x.Id == new Guid(request.AccountId));
            if (account.StatusCode == Domain.Enums.StatusCode.EntityNotFound)
            {

                return await Task.FromResult(new AthorPublicProfileReply
                {
                    Login = "-",
                    Status = "not exist",
                    CreateDate = Timestamp.FromDateTimeOffset(DateTime.Now)
                });
            }

            var reply = new AthorPublicProfileReply
            {
                Login = account.Data.Login,
                Status = account.Data.StatusAccount.ToString(),
                CreateDate = Timestamp.FromDateTimeOffset(account.Data.CreateDate)
            };

            await _redisLockService.SetString(reply, account.Data.Id.ToString());

            return await Task.FromResult(reply);
        }
    }
}

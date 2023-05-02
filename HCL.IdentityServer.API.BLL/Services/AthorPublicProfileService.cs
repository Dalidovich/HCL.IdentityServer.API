using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AthorPublicProfileService : AthorPublicProfile.AthorPublicProfileBase
    {
        private readonly IAccountService _accountService;
        private readonly IDistributedCache _distributedCache;

        public AthorPublicProfileService(IAccountService accountService, IDistributedCache cache)
        {
            _accountService = accountService;
            _distributedCache = cache;
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
            var accountString = JsonSerializer.Serialize(reply);
            await _distributedCache.SetStringAsync(account.Data.Id.ToString(), accountString, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

            return await Task.FromResult(reply);
        }
    }
}

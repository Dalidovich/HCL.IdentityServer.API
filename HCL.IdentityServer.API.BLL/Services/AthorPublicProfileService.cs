using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.Domain.DTO.Builders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AthorPublicProfileService : AthorPublicProfile.AthorPublicProfileBase
    {
        private readonly IAccountService _accountService;
        private readonly IRedisLockService _redisLockService;
        private readonly ILogger<AthorPublicProfileService> _logger;

        public AthorPublicProfileService(IAccountService accountService, IRedisLockService redisLockService
            , ILogger<AthorPublicProfileService> logger)
        {
            _accountService = accountService;
            _redisLockService = redisLockService;
            _logger = logger;
        }

        public override async Task<AthorPublicProfileReply> GetProfile(AthorIdRequest request, ServerCallContext context)
        {
            var log = new LogDTOBuidlder("GetProfile(request,context)");
            var account = await _accountService.GetAccount(x => x.Id == new Guid(request.AccountId));
            if (account.StatusCode == Domain.Enums.StatusCode.EntityNotFound)
            {
                log.BuildMessage("no account");
                _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

                return await Task.FromResult(new AthorPublicProfileReply
                {
                    Login = "-",
                    Status = "not exist",
                    CreateDate = Timestamp.FromDateTimeOffset(DateTime.Now)
                });
            }
            log.BuildMessage("get account");
            _logger.LogInformation(JsonSerializer.Serialize(log.Build()));

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

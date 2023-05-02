using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HCL.IdentityServer.API.BLL.gRPCServices;
using HCL.IdentityServer.API.BLL.Interfaces;

namespace HCL.IdentityServer.API.BLL.Services
{
    public class AthorPublicProfileService : AthorPublicProfile.AthorPublicProfileBase
    {
        private readonly IAccountService _accountService;

        public AthorPublicProfileService(IAccountService accountService)
        {
            _accountService = accountService;
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

            return await Task.FromResult(new AthorPublicProfileReply
            {
                Login = account.Data.Login,
                Status = account.Data.StatusAccount.ToString(),
                CreateDate = Timestamp.FromDateTimeOffset(account.Data.CreateDate)
            });
        }
    }
}

using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HCL.IdentityServer.API.HostedService
{
    public class GrpcServerStartup
    {
        private readonly IConfiguration _configuration;

        public GrpcServerStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddDbContext<AppDBContext>(opt => opt.UseNpgsql(
                _configuration.GetConnectionString(StandartConst.NameConnection)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AthorPublicProfileService>();
            });
        }
    }
}
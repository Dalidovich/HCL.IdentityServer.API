using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.DTO;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
            services.AddScoped<IRedisLockService, RedisLockService>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _configuration.GetSection("RedisOptions:Host").Value;
            });

            services.Configure<RedisOptions>(_configuration.GetSection("RedisOptions"));
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<RedisOptions>>().Value);

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
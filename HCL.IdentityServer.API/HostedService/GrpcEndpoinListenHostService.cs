using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace HCL.IdentityServer.API.HostedService
{
    public class GrpcEndpoinListenHostService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .ConfigureKestrel(options =>
                        {
                            options.Listen(IPAddress.Any, 5001, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                        })
                        .UseKestrel()
                        .UseStartup<GrpcServerStartup>();
                })
                .Build()
                .StartAsync(stoppingToken);
        }
    }
}
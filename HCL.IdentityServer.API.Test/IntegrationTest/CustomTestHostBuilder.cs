using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.Domain.Entities;
using HCL.IdentityServer.API.Domain.Enums;
using HCL.IdentityServer.API.HostedService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HCL.IdentityServer.API.Test.IntegrationTest
{
    public class CustomTestHostBuilder
    {
        public static WebApplicationFactory<Program> BuildWithAdmin(string dbUser, string dbPassword, string dbServer, ushort dbPort, string dbName)
        {
            var npgsqlConnectionString = $"User Id={dbUser}; Password={dbPassword}; Server={dbServer}; " +
                $"Port={dbPort}; Database={dbName}; IntegratedSecurity=true; Pooling=true";

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(async services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d =>
                             d.ServiceType == typeof(DbContextOptions<AppDBContext>));

                    services.Remove(dbContextDescriptor);

                    services.AddDbContext<AppDBContext>(options =>
                    {
                        options.UseNpgsql(npgsqlConnectionString);
                    });

                    var grpc = services.SingleOrDefault(d =>
                             d.ImplementationType == typeof(GrpcEndpoinListenHostService));

                    services.Remove(grpc);

                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<AppDBContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    await context.Accounts.AddAsync(new Account()
                    {
                        Login = "admin",
                        StatusAccount = StatusAccount.normal,
                        Role = Role.admin,
                        CreateDate = DateTime.Now,
                        Salt = "QTPZeLPujIVXf66A2Ggu0YE4LS2ZC7lZiDd6WGlkpiAyh05WTRmXhRjXZzf90+zrusy65s7ghCnwna6F5A/1swR/xn1VxEgEsrKaVM+vxaST6OwNRjks2Tb6elCAyOEFrplcdGR/JbjxwhO/A1VZqL+259h7CdxvCu6KFc5kZgI=",
                        Password = "5a2CBsDoaFXUG+p+QRrrQlH520CrvNnVIAEEki1Sl0gKiugBx6qyqBMyfriO0+yyg41gIGqofvRn3UZbQn+xyg==",
                    });

                    await context.SaveChangesAsync();
                });
            });
        }
    }
}

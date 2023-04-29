using Google.Protobuf.WellKnownTypes;
using HCL.IdentityServer.API.BLL.Interfaces;
using HCL.IdentityServer.API.BLL.Services;
using HCL.IdentityServer.API.DAL.Repositories;
using HCL.IdentityServer.API.DAL.Repositories.Interfaces;
using HCL.IdentityServer.API.Domain.JWT;
using HCL.IdentityServer.API.Midlaware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HCL.IdentityServer.API
{
    public static class DIManger
    {
        public static void AddRepositores(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IAccountRepository, AccountRepository>();
        }

        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IAccountService, AccountService>();
            webApplicationBuilder.Services.AddScoped<IRegistrationService, RegistrationService>();
            webApplicationBuilder.Services.AddScoped<ITokenService, TokenService>();
        }

        public static void AddHostedService(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHostedService<GrpcEndpoinListenHostService>();
        }

        public static void AddJWT(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<JWTSettings>(webApplicationBuilder.Configuration.GetSection("JWTSettings"));
            var secretKey = webApplicationBuilder.Configuration.GetSection("JWTSettings:SecretKey").Value;
            var issuer = webApplicationBuilder.Configuration.GetSection("JWTSettings:Issuer").Value;
            var audience = webApplicationBuilder.Configuration.GetSection("JWTSettings:Audience").Value;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            webApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {

                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = JwtHelper.CustomLifeTimeValidator
                };
            });
        }

        public static void AddMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
            webApplication.UseMiddleware<CheckDBMiddleware>();
        }

        public static void AddGrpcService(this WebApplication webApplication)
        {
            webApplication.MapGrpcService<AthorPublicProfileService>();
        }
    }
}
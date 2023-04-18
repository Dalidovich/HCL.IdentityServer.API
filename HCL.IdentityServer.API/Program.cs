using HCL.IdentityServer.API.BLL.Midlaware;
using HCL.IdentityServer.API.DAL;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HCL.IdentityServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddSingleton(builder.Configuration);
            builder.AddRepositores();
            builder.AddServices();
            builder.AddJWT();

            builder.Services.AddDbContext<AppDBContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString(StandartConst.NameConnection)));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<CheckDBMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
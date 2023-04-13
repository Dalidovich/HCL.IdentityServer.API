using HCL.IdentityServer.API.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HCL.IdentityServer.API.BLL.Midlaware
{
    public class CheckDBMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CheckDBMiddleware> _logger;

        public CheckDBMiddleware(RequestDelegate next,
            ILogger<CheckDBMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                if (!await httpContext.RequestServices.GetService<AppDBContext>().Accounts.AnyAsync())
                {
                    httpContext.RequestServices.GetService<AppDBContext>().UpdateDatabase();
                }
            }
            catch (PostgresException ex)
            {
                if (ex.Code == "42P01")
                {
                    httpContext.RequestServices.GetService<AppDBContext>().UpdateDatabase();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[db update middleware]ex.Message");

                HttpResponse response = httpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = 500;

                await response.WriteAsJsonAsync(new { Message = ex.Message });
            }
            await _next.Invoke(httpContext);
        }
    }
}

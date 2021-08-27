using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Data;
using Hospital.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Hospital.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class DoctorInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public DoctorInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = db.Users.Find(userId);
            bool isInfoExist = db.Doctors.Any(u => u.UserId == userId);
            if (httpContext.User.Identity.IsAuthenticated &&
                userManager.IsInRoleAsync(user, SD.DoctorEndUser).Result &&
                !isInfoExist)
            {
                if (httpContext.Request.Path != "/CompleteDoctorData")
                {
                    httpContext.Response.Redirect("/CompleteDoctorData");
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DoctorInfoMiddlewareExtensions
    {
        public static IApplicationBuilder UseDoctorInfoMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DoctorInfoMiddleware>();
        }
    }
}

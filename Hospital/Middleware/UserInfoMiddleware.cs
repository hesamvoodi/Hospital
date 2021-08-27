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
    public class UserInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = db.Users.Find(userId);
            bool isInfoExist = db.UserInfos.Any(u => u.UserId == userId);
            if (httpContext.User.Identity.IsAuthenticated &&
                userManager.IsInRoleAsync(user, SD.PatientEndUser).Result &&
                !isInfoExist)
            {
                if (httpContext.Request.Path != "/CompleteUserData" && 
                    httpContext.Request.Path != "/UserPanel/DoctorRequest" &&
                    httpContext.Request.Path != "/Logout")
                {
                    httpContext.Response.Redirect("/CompleteUserData");
                    return Task.CompletedTask;
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UserInfoMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserInfoMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserInfoMiddleware>();
        }
    }
}

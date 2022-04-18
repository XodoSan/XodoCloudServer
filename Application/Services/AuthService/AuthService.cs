using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.AuthService
{
    public class AuthService: IAuthService
    {
        public async Task Authenticate(string email, HttpContext httpContext)
        {
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, email) };
            ClaimsIdentity id = new ClaimsIdentity(
            claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}

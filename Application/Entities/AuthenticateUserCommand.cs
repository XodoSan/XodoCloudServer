using Microsoft.AspNetCore.Http;

namespace Application.Entities
{
    public class AuthenticateUserCommand
    {
        public AuthenticateUserCommand(string email, string password, HttpContext httpContext)
        {
            Email = email;
            Password = password;
            HttpContext = httpContext;
        }

        public string Email { get; }
        public string Password { get; }
        public HttpContext HttpContext { get; }
    }
}

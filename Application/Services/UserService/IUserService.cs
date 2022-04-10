using Application.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public interface IUserService
    {
        public Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand);
        public Task<UserAuthenticationResult> Register(User user);
        public void FinishRegistration(AuthenticateUserCommand authenticateUserCommand);
        public Task<UserAuthenticationResult> ChangePassword(HttpContext httpContext, string lastPassword, string newPassword);
    }
}

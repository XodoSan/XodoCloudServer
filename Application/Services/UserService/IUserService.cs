using Application.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public interface IUserService
    {
        public Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand);
        public Task<UserAuthenticationResult> CheckToRegistration(User user);
        public void FinishRegistration(AuthenticateUserCommand authenticateUserCommand);
        public Task<UserAuthenticationResult> CheckToChangePassword(HttpContext httpContext, string lastPassword, string newPassword);
        public Task<bool> isPasswordHasChanged(string thisUserEmail, string emailHash, string newPasswordHash);
    }
}

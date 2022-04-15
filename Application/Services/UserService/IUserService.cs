using Application.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public interface IUserService
    {
        public Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand);
        public UserAuthenticationResult CheckToRegistration(User user);
        public Task FinishRegistration(AuthenticateUserCommand authenticateUserCommand);
        public UserAuthenticationResult CheckToChangePassword(HttpContext httpContext, string lastPassword);
        public bool IsPasswordChangedHasConfirmed(string thisUserEmail, string emailHash, string newPasswordHash);
    }
}

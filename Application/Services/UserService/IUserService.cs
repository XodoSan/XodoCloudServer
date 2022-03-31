using Application.Entities;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public interface IUserService
    {
        public Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand);
    }
}

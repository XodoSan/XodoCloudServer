using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Services.AuthService
{
    public interface IAuthService
    {
        public Task Authenticate(string email, HttpContext httpContext);
    }
}

using Application.Entities;
using Application.Services.FileService;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        public UserService(IUserRepository userRepository, IFileService fileService)
        {
            _userRepository = userRepository;
            _fileService = fileService;
        }

        public async Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand)
        {
            User user = await _userRepository.GetUserByEmail(authenticateUserCommand.Email);

            if (user == null)
            {
                return new UserAuthenticationResult(false, "user");
            }

            if (HashService.GetHash(authenticateUserCommand.Password) != user.PasswordHash)
            {
                return new UserAuthenticationResult(false, "password");
            }

            await Authenticate(authenticateUserCommand.Email, authenticateUserCommand.HttpContext);

            return new UserAuthenticationResult(true, null);
        }

        public async Task<UserAuthenticationResult> Register(AuthenticateUserCommand authenticateUserCommand)
        {
            User user = await _userRepository.GetUserByEmail(authenticateUserCommand.Email);

            if (user != null)
            {
                return new UserAuthenticationResult(false, "user");
            }

            User newUser = null;
            newUser = new User { Email = authenticateUserCommand.Email, PasswordHash = authenticateUserCommand.Password };

            _fileService.AddUserFolder(newUser);
            _userRepository.AddUser(newUser);

            await Authenticate(authenticateUserCommand.Email, authenticateUserCommand.HttpContext);

            return new UserAuthenticationResult(true, null);
        }

        private static async Task Authenticate(string email, HttpContext httpContext)
        {
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, email) };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}

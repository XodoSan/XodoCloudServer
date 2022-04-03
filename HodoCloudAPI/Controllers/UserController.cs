using Application;
using Application.Entities;
using Application.Services.CacheService;
using Application.Services.UserService;
using Domain.Entities;
using HodoCloudAPI.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HodoCloudAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UserController
        (
            IUserService userService,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        [HttpPost("registration")]
        public async Task<UserAuthenticationResultDto> RegisterUser([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand(authenticateUserDto);
            UserAuthenticationResult result = await _userService.Register(authenticateUserCommand);

            _unitOfWork.Commit();
            if (_unitOfWork.IsSuccessCommited())
            {
                _cacheService.SetAddedUserCache(
                    new User { Email = authenticateUserCommand.Email, PasswordHash = authenticateUserCommand.Password });
            }

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpPost("login")]
        public async Task<UserAuthenticationResultDto> Login([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand(authenticateUserDto);
            UserAuthenticationResult result = await _userService.Login(authenticateUserCommand);

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private AuthenticateUserCommand ConvertToAuthenticateUserCommand(AuthenticateUserCommandDto authenticateUserCommandDto)
        {
            return new AuthenticateUserCommand
            (
                authenticateUserCommandDto.Email,
                authenticateUserCommandDto.Password,
                HttpContext
            );
        }
    }
}

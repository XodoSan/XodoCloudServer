using Application;
using Application.Entities;
using Application.Services.UserService;
using Domain.Entities;
using Domain.Repositories;
using HodoCloudAPI.Converters;
using HodoCloudAPI.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HodoCloudAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public UserController
        (
            IUserRepository userRepository, 
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("registration")]
        public void RegisterUser([FromBody] UserDto userDto)
        {
            User user = UserConverter.ConvertToUser(userDto);
            _userRepository.AddUser(user);
            _unitOfWork.Commit();
        }

        [HttpPost("login")]
        public async Task<UserAuthenticationResultDto> Login([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand(authenticateUserDto);
            UserAuthenticationResult result = await _userService.Login(authenticateUserCommand);

            return new UserAuthenticationResultDto( result.Result, result.Error);
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
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

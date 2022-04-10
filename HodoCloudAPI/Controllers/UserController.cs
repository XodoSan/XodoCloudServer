using Application;
using Application.Entities;
using Application.Services;
using Application.Services.EmailSenderService;
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

        public UserController
        (
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("registration")]
        public async Task<UserAuthenticationResultDto> RegisterUser([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            UserAuthenticationResult result = await _userService.Register(new User 
            {
                Email = authenticateUserDto.Email, 
                PasswordHash = authenticateUserDto.Password 
            });

            await EmailSender.SendEmailAsync(authenticateUserDto.Email);

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpGet("confirm_registration/{userEmail}/{hashEmail}")]
        public string ConfirmedRegistration([FromRoute] string userEmail, [FromRoute] string hashEmail)
        {
            int substringIndex = hashEmail.IndexOf(Configuration.randomWord);
            hashEmail = hashEmail.Remove(substringIndex, Configuration.randomWord.Length);

            if (HashService.GetHash(userEmail) == hashEmail)
            {
                AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand
                    (new AuthenticateUserCommandDto 
                    { 
                        Email = userEmail, 
                        Password = "" 
                    });

                _userService.FinishRegistration(authenticateUserCommand);
                //_unitOfWork.Commit();
            }

            return "";
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
        
        [HttpGet("is_authorized")]
        public bool IsUserAuthorized()
        {
            return HttpContext.User.Identity.IsAuthenticated;
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

﻿using Application;
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
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public UserController
        (
            IUserService userService,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("registration")]
        public async Task<UserAuthenticationResultDto> CheckToRegistration([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            UserAuthenticationResult result = _userService.CheckToRegistration(new User 
            {
                Email = authenticateUserDto.Email, 
                PasswordHash = authenticateUserDto.Password 
            });

            string confirmLink = _emailSender.GenereteEmailConfirmLink(authenticateUserDto.Email);
            await _emailSender.SendEmailAsync(authenticateUserDto.Email, confirmLink);

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpGet("confirm_registration/{hashEmail}")]
        public string ConfirmRegistration([FromRoute] string hashEmail)
        {
            int substringIndex = hashEmail.IndexOf(Configuration.randomWord);
            hashEmail = hashEmail.Remove(substringIndex, Configuration.randomWord.Length);

            string thisUserEmail = Configuration.user.Email; 
            if (HashService.GetHash(thisUserEmail) == hashEmail)
            {
                User user = Configuration.user;

                AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand
                    (new AuthenticateUserCommandDto 
                    { 
                        Email = user.Email, 
                        Password = user.PasswordHash 
                    });

                _userService.FinishRegistration(authenticateUserCommand);
                _unitOfWork.Commit();

                return "Successfuly registration!";
            }

            return "Failed registration!";
        }

        [HttpPost("login")]
        public async Task<UserAuthenticationResultDto> Login([FromBody] AuthenticateUserCommandDto authenticateUserDto)
        {
            AuthenticateUserCommand authenticateUserCommand = ConvertToAuthenticateUserCommand(authenticateUserDto);
            UserAuthenticationResult result = await _userService.Login(authenticateUserCommand);

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpPost("change_password")]
        public async Task<UserAuthenticationResultDto> CheckingToChangePassword([FromBody] UserPasswordsDto userPasswordsDto)
        {
            UserAuthenticationResult result = _userService.CheckToChangePassword(
                HttpContext, userPasswordsDto.LastPassword);

            string userEmailHash = HashService.GetHash(HttpContext.User.Identity.Name);
            string newPasswordHash = HashService.GetHash(userPasswordsDto.NewPassword);

            string confirmLink = _emailSender.GeneratePasswordConfirmLink(userEmailHash, newPasswordHash);
            await _emailSender.SendEmailAsync(HttpContext.User.Identity.Name, confirmLink);

            return new UserAuthenticationResultDto(result.Result, result.Error);
        }

        [HttpGet("confirm_change_password/{emailHash}/{newPasswordHash}")]
        public string ConfirmChangePassword([FromRoute] string emailHash, [FromRoute] string newPasswordHash)
        {
            int substringIndex = newPasswordHash.IndexOf(Configuration.randomWord);
            newPasswordHash = newPasswordHash.Remove(substringIndex, Configuration.randomWord.Length);

            string thisUserEmail = HttpContext.User.Identity.Name;

            if (_userService.IsPasswordChangedHasConfirmed(thisUserEmail, emailHash, newPasswordHash))
            {
                _unitOfWork.Commit();

                return "Password is changed!";
            }

            return "Password has not been changed";
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        
        [HttpGet("is_authorized")]
        public string IsUserAuthorized()
        {
            return HttpContext.User.Identity.Name;
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

using Application.Entities;
using Application.Services;
using Application.Services.AuthService;
using Application.Services.FileService;
using Application.Services.UserService;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class UserServiceTest
    {
        private static readonly string defaultUserPassword = "";
        private static readonly string defaultUserEmail = "XodoSan";

        private readonly IUserRepository _userRepository = Mock.Of<IUserRepository>(method => method.
            GetUserByEmail(It.IsAny<string>()) == new User 
            { 
                Email = defaultUserEmail, PasswordHash = HashService.GetHash(defaultUserPassword) 
            });

        private readonly IAuthService _authService = Mock.Of<IAuthService>(method => method.
            Authenticate(It.IsAny<string>(), It.IsAny<HttpContext>()) == Task.WhenAll());

        private readonly IFileService _fileService = Mock.Of<IFileService>();
        private readonly IUserService _userService;

        public UserServiceTest()
        {
            _userService = new UserService(_userRepository, _fileService, _authService);
        }

        [Fact]
        public void CheckToRegistration_ShouldReturnFalse()
        {
            UserAuthenticationResult hypothesis = new UserAuthenticationResult(false, "user");

            User user = new User { Email = defaultUserEmail, PasswordHash = defaultUserPassword };
            UserAuthenticationResult result = _userService.CheckToRegistration(user);

            Assert.Equal(hypothesis.Result, result.Result);
        }

        [Fact]
        public async void Login_ShouldReturnTrue()
        {
            UserAuthenticationResult hypothesis = new UserAuthenticationResult(true, null);

            DefaultHttpContext httpContext = new DefaultHttpContext();
            AuthenticateUserCommand userCommand = new AuthenticateUserCommand(defaultUserEmail, defaultUserPassword, httpContext);

            UserAuthenticationResult result = await _userService.Login(userCommand);

            Assert.Equal(hypothesis.Result, result.Result);
        }

        [Fact]
        public async void FinishRegistration_ShouldReturnVoid()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            AuthenticateUserCommand userCommand = new AuthenticateUserCommand(defaultUserEmail, defaultUserPassword, httpContext);

            await _userService.FinishRegistration(userCommand);
        }

        [Fact]
        public void CheckToChangePassword_ShouldReturnTrue()
        {
            UserAuthenticationResult hypothesis = new UserAuthenticationResult(true, null);

            DefaultHttpContext httpContext = new DefaultHttpContext();
            UserAuthenticationResult result = _userService.CheckToChangePassword(httpContext, defaultUserPassword);

            Assert.Equal(hypothesis.Result, result.Result);
        }

        [Fact]
        public void IsPasswordChangedHasConfirmed_ShouldReturnTrue()
        {
            bool hypothesis = true;
            bool result = _userService.IsPasswordChangedHasConfirmed(defaultUserEmail, HashService.GetHash(defaultUserEmail), "");

            Assert.Equal(hypothesis, result);
        }
    }
}

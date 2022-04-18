using Application.Entities;
using Application.Services.AuthService;
using Application.Services.FileService;
using Application.Services.HashService;
using Application.Services.UserService;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class UserServiceTests
    {
        private const string defaultUserEmail = "XodoSan";
        private const string defaultUserPassword = "test";

        private static readonly IHashService _hashService = new HashService();

        private readonly IUserRepository _userRepository = Mock.Of<IUserRepository>(method => method.
            GetUserByEmail(It.IsAny<string>()) == new User 
            { 
                Email = defaultUserEmail, PasswordHash = _hashService.GetHash(defaultUserPassword) 
            });

        private readonly IAuthService _authService = Mock.Of<IAuthService>(method => method.
            Authenticate(It.IsAny<string>(), It.IsAny<HttpContext>()) == Task.WhenAll());

        private readonly IFileService _fileService = Mock.Of<IFileService>();
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService(_userRepository, _fileService, _authService, _hashService);
        }

        [Fact]
        public void CheckToRegistration_ShouldReturnFalse()
        {
            UserAuthenticationResult hypothesis = new UserAuthenticationResult(false, "user");

            User user = new User { Email = defaultUserEmail, PasswordHash = defaultUserPassword };
            UserAuthenticationResult result = _userService.CheckToRegistration(user);

            Assert.Equal(hypothesis.Result, result.Result);
        }

        [Theory]
        [InlineData(defaultUserEmail, defaultUserPassword, true)]
        [InlineData(defaultUserEmail, "", false)]
        public async void Login_Test(string userEmail, string userPassword, bool hypothesis)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            AuthenticateUserCommand userCommand = new AuthenticateUserCommand(userEmail, userPassword, httpContext);

            UserAuthenticationResult result = await _userService.Login(userCommand);

            Assert.Equal(hypothesis, result.Result);
        }

        [Fact]
        public async void FinishRegistration_ShouldReturnVoid()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            AuthenticateUserCommand userCommand = new AuthenticateUserCommand(defaultUserEmail, defaultUserPassword, httpContext);

            await _userService.FinishRegistration(userCommand);
        }

        [Theory]
        [InlineData(defaultUserPassword, true)]
        [InlineData("", false)]
        public void CheckToChangePassword_Test(string userPassword, bool hypothesis)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            UserAuthenticationResult result = _userService.CheckToChangePassword(httpContext, userPassword);

            Assert.Equal(hypothesis, result.Result);
        }

        [Theory]
        [InlineData(defaultUserEmail, defaultUserEmail, true)]
        [InlineData(defaultUserEmail, "", false)]
        public void IsPasswordChangedHasConfirmed_Test(string userEmail, string userEmailHash, bool hypothesis)
        {
            userEmailHash = _hashService.GetHash(userEmailHash);

            bool result = _userService.IsPasswordChangedHasConfirmed(userEmail, userEmailHash, "");

            Assert.Equal(hypothesis, result);
        }
    }
}

using Application.Entities;
using Application.Services.UserService;
using Domain.Entities;
using Xunit;

namespace HodoCloudAPI.Tests
{
    public class UserServiceTest
    {
        IUserService _userService;

        public UserServiceTest()
        {
            
        }

        [Fact]
        public async void CheckToRegistration_ShouldReturnedFalseAndUser()
        {
            UserAuthenticationResult hypothesis = new(false, "user");

            User user = new User { Email = "test", PasswordHash = "test" };
            UserAuthenticationResult result = await _userService.CheckToRegistration(user);

            Assert.Equal(hypothesis, result);
        }
    }
}

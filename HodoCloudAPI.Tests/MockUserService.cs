using Application;
using Application.Entities;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HodoCloudAPI.Tests
{
    public static class MockUserService
    {
        public async Task<UserAuthenticationResult> CheckToRegistration(User user)
        {
            List<User> mockedUsers = new();
            mockedUsers.Add(new User { Email = "Admin", PasswordHash = "" });

            User checkUser = mockedUsers.Where(user => user.Email == user.Email).FirstOrDefault();

            if (checkUser != null)
            {
                return new UserAuthenticationResult(false, "user");
            }

            return new UserAuthenticationResult(true, null);
        }
    }
}

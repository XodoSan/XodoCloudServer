using Domain.Entities;
using HodoCloudAPI.Dtos;

namespace HodoCloudAPI.Converters
{
    public static class UserConverter
    {
        public static User ConvertToUser(UserDto userDto)
        {
            return new User()
            {
                Email = userDto.Email,
                PasswordHash = userDto.Password
            };
        }
    }
}

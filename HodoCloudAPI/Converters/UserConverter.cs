using Domain.Entities;
using HodoCloudAPI.Dtos;

namespace HodoCloudAPI.Converters
{
    public static class UserConverter
    {
        public static User ConvertToUser(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                PasswordHash = userDto.Password
            };
        }
    }
}

using Application.DtoEntities;
using Domain.Entities;

namespace Application.Services.ConvertService
{
    public class ConvertService: IConvertService
    {
        public User ConvertToUser(UserDto userDto)
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

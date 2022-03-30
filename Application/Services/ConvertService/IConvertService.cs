using Application.DtoEntities;
using Domain.Entities;

namespace Application.Services.ConvertService
{
    public interface IConvertService
    {
        public User ConvertToUser(UserDto userDto);
    }
}

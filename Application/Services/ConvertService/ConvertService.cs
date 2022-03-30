using Application.DtoEntities;
using Application.Services.HashService;
using Domain.Entities;

namespace Application.Services.ConvertService
{
    public class ConvertService: IConvertService
    {
        private readonly IHashService _hashService;

        public ConvertService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public User ConvertToUser(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                PasswordHash = _hashService.GetHash(userDto.Password)
            };
        }
    }
}

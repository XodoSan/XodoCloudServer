using Application;
using Application.DtoEntities;
using Application.Services.ConvertService;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HodoCloudAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConvertService _convertService;
        private readonly IUnitOfWork _unitOfWork;

        public UserController
        (
            IUserRepository userRepository, 
            IConvertService convertService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _convertService = convertService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Registration")]
        public void AddUser([FromBody] UserDto userDto)
        {
            User user = _convertService.ConvertToUser(userDto);
            _userRepository.AddUser(user);
            _unitOfWork.Commit();
        }

        [HttpGet]
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }
    }
}

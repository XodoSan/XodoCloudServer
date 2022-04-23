using Application.Entities;
using Application.Services.AuthService;
using Application.Services.FileService;
using Application.Services.HashService;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Services.UserService
{
    public class UserService: IUserService
    {
        public static string stubData;

        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;
        private readonly IHashService _hashService;

        public UserService(
            IUserRepository userRepository, 
            IFileService fileService, 
            IAuthService authService,
            IHashService hashService)
        {
            _userRepository = userRepository;
            _fileService = fileService;
            _authService = authService;
            _hashService = hashService;
        }

        public async Task<UserAuthenticationResult> Login(AuthenticateUserCommand authenticateUserCommand)
        {
            User user = _userRepository.GetUserByEmail(authenticateUserCommand.Email);

            if (user == null)
            {
                return new UserAuthenticationResult(false, "user");
            }

            if (_hashService.GetHash(authenticateUserCommand.Password) != user.PasswordHash)
            {
                return new UserAuthenticationResult(false, "password");
            }

            await _authService.Authenticate(authenticateUserCommand.Email, authenticateUserCommand.HttpContext);

            return new UserAuthenticationResult(true, null);
        }

        public UserAuthenticationResult CheckToRegistration(User user)
        {
            User checkUser = _userRepository.GetUserByEmail(user.Email);

            if (checkUser != null)
            {
                return new UserAuthenticationResult(false, "user");
            }

            Configuration.user = user;

            return new UserAuthenticationResult(true, null);
        }

        public async Task FinishRegistration(AuthenticateUserCommand authenticateUserCommand)
        {
            stubData = authenticateUserCommand.Email;
            _fileService.AddUserFolder(authenticateUserCommand.Email);

            string passwordHash = _hashService.GetHash(authenticateUserCommand.Password);
            _userRepository.AddUser(new User {Email = authenticateUserCommand.Email, PasswordHash = passwordHash });

            await _authService.Authenticate(authenticateUserCommand.Email, authenticateUserCommand.HttpContext);
        }

        public UserAuthenticationResult CheckToChangePassword(HttpContext httpContext, string lastPassword)
        {
            string userEmail = Configuration.user.Email;
            User thisUser = _userRepository.GetUserByEmail(userEmail);

            if (thisUser.PasswordHash != _hashService.GetHash(lastPassword))
            {
                return new UserAuthenticationResult(false, "password");
            }

            return new UserAuthenticationResult(true, null);
        }

        public bool IsPasswordChangedHasConfirmed(string thisUserEmail, string emailHash, string newPasswordHash)
        {
            User thisUser = _userRepository.GetUserByEmail(thisUserEmail);

            if (emailHash == _hashService.GetHash(thisUserEmail))
            {
                thisUser.UpdatePassword(newPasswordHash);
                return true;
            }

            return false;
        }
    }
}

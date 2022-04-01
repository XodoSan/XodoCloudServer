using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services.FileService
{
    public interface IFileService
    {
        public bool ValidateFile(IFormFile userFile);
        public void AddUserFolder(User user);
    }
}

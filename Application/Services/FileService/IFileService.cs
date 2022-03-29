using Microsoft.AspNetCore.Http;

namespace Application.Services.FileService
{
    public interface IFileService
    {
        public bool ValidateFile(IFormFile userFile);
    }
}

using Microsoft.AspNetCore.Http;

namespace Application.Services.FileService
{
    public class FileService: IFileService
    {
        private int maxFileSize = 524288000;

        public bool ValidateFile(IFormFile userFile)
        {
            if (userFile.Length < maxFileSize)
            {
                return true;
            }

            return false;
        }
    }
}

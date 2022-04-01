using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Application.Services.FileService
{
    public class FileService: IFileService
    {
        private int maxFileSize = 524288000;
        private string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";

        public bool ValidateFile(IFormFile userFile)
        {
            if (userFile.Length < maxFileSize)
            {
                return true;
            }

            return false;
        }

        public void AddUserFolder(User user)
        {
            Directory.CreateDirectory(basePath + user.Email);
        }
    }
}

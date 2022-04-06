using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public List<string> GetUserFileNames(string userEmail)
        {
            string userFolderPath = basePath + userEmail;
            List<string> result = new();
            result = Directory.GetFiles(userFolderPath).ToList();

            //userFolderPath.Length + 1. Plus 1 remove '\' simbol
            return result.Select(result => result.Remove(0, userFolderPath.Length + 1)).ToList();
        }

        public void DeleteUserFiles(string userEmail, string[] userFileNames)
        {
            for (int i = 0; i < userFileNames.Length; i++)
            {
                File.Delete(basePath + userEmail + @"\" + userFileNames[i]);
            }
        }

        public async Task<FileResult> DownloadUserFile(string userEmail, string userFileName)
        {
            string userFolderPath = basePath + userEmail + @"\";

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(userFileName, out contentType);

            byte[] result = await File.ReadAllBytesAsync(userFolderPath + userFileName);

            var fileContentResult = new FileContentResult(result, contentType)
            {
                FileDownloadName = userFileName
            };

            return fileContentResult;
        }
    }
}

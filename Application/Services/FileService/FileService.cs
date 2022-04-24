using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.FileService
{
    public class FileService: IFileService
    {
        public static string stubEmail; //this variable created for tests
        public static string stubFileName; //this variable created for tests
        private int maxFileSize = 524288000;
        public static string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";

        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public bool ValidateFile(long fileLength)
        {
            if (fileLength < maxFileSize)
            {
                return true;
            }

            return false;
        }

        public void PostUserFile(IFormFile userFile, HttpContext httpContext)
        {
            if (ValidateFile(userFile.Length))
            {
                stubFileName = userFile.FileName;
                _fileRepository.SaveFileToUserFolder(userFile, httpContext.User.Identity.Name);
            }
        }

        public void AddUserFolder(string userEmail)
        {
            stubEmail = userEmail;
            _fileRepository.AddUserFolder(userEmail);
        }

        public List<string> GetUserFileNames(string userEmail)
        {
            StringBuilder userFolderPath = new();
            userFolderPath.Append(basePath).Append(userEmail);

            List<string> result = new();
            result = _fileRepository.GetFilePathsFromUserFolder(userFolderPath.ToString());

            //userFolderPath.Length + 1. Plus 1 remove '\' simbol
            return result.Select(result => result.Remove(0, userFolderPath.Length + 1)).ToList();
        }

        public void DeleteUserFiles(string userEmail, string[] userFileNames)
        {
            stubEmail = userEmail;
            StringBuilder filePath = new();

            for (int i = 0; i < userFileNames.Length; i++)
            {
                filePath.Append(basePath).Append(userEmail).Append("/").Append(userFileNames[i]);
                _fileRepository.DeleteFile(filePath.ToString());
            }
        }

        public async Task<FileResult> DownloadUserFile(string userEmail, string userFileName)
        {
            StringBuilder userFolderPath = new();
            userFolderPath.Append(basePath).Append(userEmail).Append(@"\");

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(userFileName, out contentType);

            byte[] result = await _fileRepository.ReadFile(userFolderPath + userFileName);

            var fileContentResult = new FileContentResult(result, contentType)
            {
                FileDownloadName = userFileName
            };

            return fileContentResult;
        }
    }
}

using Domain.Repositories;
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
        private readonly string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";

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

        public void AddUserFolder(string userEmail)
        {
            _fileRepository.AddUserFolder(userEmail);
        }

        public List<string> GetUserFileNames(string userEmail)
        {
            string userFolderPath = basePath + userEmail;
            List<string> result = new();
            result = _fileRepository.GetFilePathsFromUserFolder(userFolderPath);

            //userFolderPath.Length + 1. Plus 1 remove '\' simbol
            return result.Select(result => result.Remove(0, userFolderPath.Length + 1)).ToList();
        }

        public void DeleteUserFiles(string userEmail, string[] userFileNames)
        {
            for (int i = 0; i < userFileNames.Length; i++)
            {
                string filePath = basePath + userEmail + @"\" + userFileNames[i];
                _fileRepository.DeleteFile(filePath);
            }
        }

        public async Task<FileResult> DownloadUserFile(string userEmail, string userFileName)
        {
            string userFolderPath = basePath + userEmail + @"\";

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

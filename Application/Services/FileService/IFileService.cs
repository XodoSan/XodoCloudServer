using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.FileService
{
    public interface IFileService
    {
        public bool ValidateFile(long fileLength);
        public void PostUserFile(IFormFile userFile, HttpContext httpContext);
        public void AddUserFolder(string userEmail);
        public List<string> GetUserFileNames(string userEmail);
        public void DeleteUserFiles(string userEmail, string[] userFiles);
        public Task<FileResult> DownloadUserFile(string userEmail, string userFileName);
    }
}

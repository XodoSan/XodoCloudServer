using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Application.Services.FileService
{
    public interface IFileService
    {
        public bool ValidateFile(IFormFile userFile);
        public void AddUserFolder(User user);
        public List<string> GetUserFileNames(string userEmail);
    }
}

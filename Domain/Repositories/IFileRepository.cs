using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IFileRepository
    {
        public void SaveFileToUserFolder(IFormFile userFile, string authenticateUserEmail);
        public void AddUserFolder(string userEmail);
        public List<string> GetFilePathsFromUserFolder(string userFolderPath);
        public void DeleteFile(string filePath);
        public Task<byte[]> ReadFile(string filePath);
    }
}
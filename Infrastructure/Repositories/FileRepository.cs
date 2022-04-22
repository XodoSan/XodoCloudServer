using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileRepository: IFileRepository
    {
        public static string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";

        public void SaveFileToUserFolder(IFormFile userFile, string authenticateUserEmail)
        {
            using
            (
                Stream fileStream = new FileStream(
                basePath + authenticateUserEmail + @"\" + userFile.FileName, FileMode.Create)
            )
            {
                userFile.CopyToAsync(fileStream);
                fileStream.Close();
            }
        }

        public void AddUserFolder(string userEmail)
        {
            Directory.CreateDirectory(basePath + userEmail);
        }

        public List<string> GetFilePathsFromUserFolder(string userFolderPath)
        {
            return Directory.GetFiles(userFolderPath).ToList();
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public async Task<byte[]> ReadFile(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }
    }
}

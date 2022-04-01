using Microsoft.AspNetCore.Http;
using System.IO;

namespace Infrastructure.Loader
{
    public class FileLoader: IFileLoader
    {
        private static string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";
        
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
    }
}
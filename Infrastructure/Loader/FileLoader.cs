using Microsoft.AspNetCore.Http;
using System.IO;

namespace Infrastructure.Loader
{
    public class FileLoader: IFileLoader
    {
        private static string basePath = Directory.GetCurrentDirectory() + @"\";
        
        public void SaveFileToUserFolder(IFormFile userFile)//User email need be into arguments and paste after Users
        {
            using (Stream fileStream = new FileStream(basePath + "Users" + @"\" + userFile.FileName, FileMode.Create))
            {
                userFile.CopyToAsync(fileStream);
                fileStream.Close();
            }
        }
    }
}
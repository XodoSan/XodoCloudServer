using Microsoft.AspNetCore.Http;
using System.IO;

namespace Infrastructure.Loader
{
    public class FileLoader: IFileLoader
    {
        private string basePath = @"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI\bin\Debug\net5.0\Users\";

        public void SaveFileToUserFolder(IFormFile userFile)
        {
            using (Stream fileStream = new FileStream(basePath + userFile.FileName, FileMode.Create))
            {
                userFile.CopyToAsync(fileStream);
                fileStream.Close();
            }
        }
    }
}
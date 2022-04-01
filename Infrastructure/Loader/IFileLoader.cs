using Microsoft.AspNetCore.Http;

namespace Infrastructure.Loader
{
    public interface IFileLoader
    {
        public void SaveFileToUserFolder(IFormFile userFile, string authenticateUserEmail);
    }
}

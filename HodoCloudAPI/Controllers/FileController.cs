using Infrastructure.Loader;
using Microsoft.AspNetCore.Mvc;

namespace HodoCloudAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase
    {
        private readonly IFileLoader _fileLoader;

        public FileController(IFileLoader fileLoader)
        {
            _fileLoader = fileLoader;
        }

        [HttpPost, DisableRequestSizeLimit]
        public void PostUserFile()
        {            
            var userFile = Request.Form.Files[0];
            _fileLoader.SaveFileToUserFolder(userFile);
        }
    }
}
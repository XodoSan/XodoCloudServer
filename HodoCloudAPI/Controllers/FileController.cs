using Infrastructure.Loader;
using Microsoft.AspNetCore.Mvc;
using Application.Services.FileService;

namespace HodoCloudAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase
    {
        private readonly IFileLoader _fileLoader;
        private readonly IFileService _fileService;

        public FileController(IFileLoader fileLoader, IFileService fileService)
        {
            _fileLoader = fileLoader;
            _fileService = fileService;
        }

        [HttpPost]
        public void PostUserFile()
        {            
            var userFile = Request.Form.Files[0];
            if (_fileService.ValidateFile(userFile))
            {
                _fileLoader.SaveFileToUserFolder(userFile);
            }
        }
    }
}
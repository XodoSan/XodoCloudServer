using Microsoft.AspNetCore.Mvc;
using Application.Services.FileService;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HodoCloudAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        [DisableRequestSizeLimit]
        public void PostUserFile()
        {
            var userFile = Request.Form.Files[0];

            if (userFile == null)
            {
                _logger.LogWarning($"Attempt to post file (none file in request), User: {HttpContext.User.Identity.Name}");
            }

            _fileService.PostUserFile(userFile, HttpContext);
        }

        [HttpPost("delete")]
        [Authorize]
        public void DeleteUserFiles([FromBody] string[] userFiles)
        {
            if (userFiles == null)
            {
                _logger.LogWarning($"Attempt to delete files (list is empty), User: {HttpContext.User.Identity.Name}");
            }

            _fileService.DeleteUserFiles(HttpContext.User.Identity.Name, userFiles);
        }

        [HttpGet]
        [Authorize]
        public List<string> GetFileNames()
        {
            return _fileService.GetUserFileNames(HttpContext.User.Identity.Name);
        }

        [HttpGet("download/{userFileName}")]
        [Authorize]
        public async Task<FileResult> DownloadFile(string userFileName)
        {
            return await _fileService.DownloadUserFile(HttpContext.User.Identity.Name, userFileName);
        }
    }
}
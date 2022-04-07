﻿using Infrastructure.Loader;
using Microsoft.AspNetCore.Mvc;
using Application.Services.FileService;
using Microsoft.AspNetCore.Authorization;
using Application.Services.UserService;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [Authorize]
        [DisableRequestSizeLimit]
        public void PostUserFile()
        {
            var userFile = Request.Form.Files[0];
            if (_fileService.ValidateFile(userFile))
            {
                _fileLoader.SaveFileToUserFolder(userFile, HttpContext.User.Identity.Name);
            }
        }

        [HttpPost("delete")]
        [Authorize]
        public void DeleteUserFiles([FromBody] string[] userFiles)
        {
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
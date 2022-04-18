using Application.Services.FileService;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class FileServiceTests
    {
        private const string defaultUserEmail = "XodoSan";
        private static string basePath = Directory.GetCurrentDirectory() + @"\" + "Users" + @"\";

        private readonly IFileRepository _fileRepository = Mock.Of<IFileRepository>(method => method.
            GetFilePathsFromUserFolder(It.IsAny<string>()) == new List<string> 
            { 
                basePath + $"{defaultUserEmail}/file.txt" 
            } && method.
            ReadFile(It.IsAny<string>()) == Task.FromResult(new byte[] { 0 }));

        private readonly IFileService _fileService;

        public FileServiceTests()
        {
            _fileService = new FileService(_fileRepository);
        }

        [Fact]
        public void ValidateFile_ShouldReturnTrue()
        {
            bool hypothesis = true;

            long fileLength = 256;
            bool result = _fileService.ValidateFile(fileLength);

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public void AddUserFolder_ShouldReturnVoid()
        {
            _fileService.AddUserFolder(defaultUserEmail);
        }

        [Fact]
        public void GetUserFileNames_ShouldReturnListString()
        {
            List<string> hypothesis = new List<string> { "file.txt" };

            List<string> result = _fileService.GetUserFileNames(defaultUserEmail);

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public void DeleteUserFiles_ShouldReturnVoid()
        {
            string[] userFiles = { "file.txt" };
            _fileService.DeleteUserFiles(defaultUserEmail, userFiles);
        }

        [Fact]
        public async void DownloadUserFile_ShouldReturnFile()
        {
            byte[] content = { 0 };
            string contentType = "text/plain";
            string userFileName = "file.txt";

            FileResult hypothesis = new FileContentResult(content, contentType)
            {
                FileDownloadName = userFileName
            };

            FileResult result = await _fileService.DownloadUserFile(defaultUserEmail, userFileName);

            Assert.Equal(hypothesis.FileDownloadName, result.FileDownloadName);
        }
    }
}

using Application.Services.FileService;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

        [Theory]
        [InlineData(256, true)]
        [InlineData(1000000000, false)]
        public void ValidateFile_Test(long fileLength, bool hypothesis)
        {
            bool result = _fileService.ValidateFile(fileLength);

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public void PostUserFile_Test()
        {
            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI\Users\test");
            using (var stream = File.OpenRead("file.txt"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };
                HttpContext httpContext = new DefaultHttpContext();

                _fileService.PostUserFile(file, httpContext);

                Assert.Equal(FileService.stubFileName, file.FileName);
            }
        }

        [Fact]
        public void AddUserFolder_Test()
        {
            _fileService.AddUserFolder(defaultUserEmail);

            Assert.Equal(defaultUserEmail, FileService.stubEmail);
        }

        [Fact]
        public void GetUserFileNames_ShouldReturnListString()
        {
            List<string> hypothesis = new List<string> { "file.txt" };

            List<string> result = _fileService.GetUserFileNames(defaultUserEmail);

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public void DeleteUserFiles_Test()
        {
            string[] userFiles = { "file.txt" };

            _fileService.DeleteUserFiles(defaultUserEmail, userFiles);

            Assert.Equal(defaultUserEmail, FileService.stubEmail);
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

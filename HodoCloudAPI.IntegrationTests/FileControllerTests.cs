using Application.Services.FileService;
using Domain.Repositories;
using HodoCloudAPI.Controllers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HodoCloudAPI.IntegrationTests
{
    public class FileControllerTests: IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly static IFileRepository _fileRepository = new FileRepository();
        private readonly IFileService _fileService = new FileService(_fileRepository);

        public FileControllerTests(TestingWebAppFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:5001/");
        }

        [Fact]
        public async Task GetFiles_Test()
        {
            FileService.basePath = @"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI\Users\";
            var controller = new FileController(_fileService);

            var controllerContext = new ControllerContext()
            {
                HttpContext = Mock.Of<HttpContext>(x => x.User.Identity.Name == "test" && x.User.Identity.IsAuthenticated == true)
            };

            controller.ControllerContext = controllerContext;

            List<string> files = controller.GetFileNames();


            //HttpResponseMessage response = await _client.GetAsync(requestUri: "api/File");
            //response.EnsureSuccessStatusCode();

            //var responseString = await response.Content.ReadAsStringAsync();

            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

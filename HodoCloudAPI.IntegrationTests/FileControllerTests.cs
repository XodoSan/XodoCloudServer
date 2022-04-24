using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using static HodoCloudAPI.IntegrationTests.UserControllerTests;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace HodoCloudAPI.IntegrationTests
{
    public class FileControllerTests: IClassFixture<TestingWebAppFactory<Startup>>
    {
        private const string defaultUserEmail = "a@yandex.ru";

        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IFileRepository _fileRepository = new FileRepository();

        public FileControllerTests(TestingWebAppFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI\Users\test");
            using (var stream = File.OpenRead("file.txt"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };
                Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI");
                _fileRepository.AddUserFolder(defaultUserEmail);
                _fileRepository.SaveFileToUserFolder(file, defaultUserEmail);
            }

            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                });
            }).CreateClient();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetFiles_Test()
        {
            List<string> hypothesis = new List<string> { "file.txt" };
            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI");

            HttpResponseMessage response = await _client.GetAsync("api/File");
            response.EnsureSuccessStatusCode();
            string jsonResult = await response.Content.ReadAsStringAsync();
            List<string> result = JsonConvert.DeserializeObject<List<string>>(jsonResult);

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public async Task PostUserFile_Test()
        {
            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI\Users\test");
            using (var stream = File.OpenRead("file.txt"))
            {
                HttpContent fileStreamContent = new StreamContent(stream);
                var formData = new MultipartFormDataContent();
                formData.Add(fileStreamContent, "file.txt", "file.txt");
                Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI");

                HttpResponseMessage response = await _client.PostAsync("api/File", formData);
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task DeleteUserFiles_Test()
        {
            var bodyContent = new string[] { "file.txt" };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI");

            HttpResponseMessage response = await _client.PostAsync("api/File/delete", httpContent);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void DownloadFile_File()
        {
            Directory.SetCurrentDirectory(@"C:\Users\Андрей\source\repos\HodoCloud\HodoCloudAPI");
            byte[] content = { 0 };
            string contentType = "text/plain";
            string userFileName = "file.txt";

            FileResult hypothesis = new FileContentResult(content, contentType)
            {
                FileDownloadName = userFileName
            };

            HttpResponseMessage response = _client.GetAsync($"api/File/download/{userFileName}").Result;
            response.EnsureSuccessStatusCode();

            Assert.Equal(hypothesis.ContentType, response.Content.Headers.ContentType.ToString());
        }
    }
}

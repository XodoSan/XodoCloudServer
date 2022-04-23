using Application;
using Application.Entities;
using Application.Services.HashService;
using Domain.Entities;
using HodoCloudAPI.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HodoCloudAPI.IntegrationTests
{
    public class UserControllerTests: IClassFixture<TestingWebAppFactory<Startup>>
    {
        private const string defaultUserEmail = "a@yandex.ru";
        private const string defaultUserPassword = "test";

        private readonly HttpClient _client;
        private readonly IHashService _hashService;
        private readonly WebApplicationFactory<Startup> _factory;

        public UserControllerTests(TestingWebAppFactory<Startup> factory)
        {
            _hashService = new HashService();
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task ConfirmRegistration_Test()
        {
            string userEmailHash = _hashService.GetHash(defaultUserEmail);
            string randomWord = "hhh";
            Configuration.randomWord = randomWord;
            Configuration.user = new User { Email = defaultUserEmail, PasswordHash = defaultUserPassword };

            HttpResponseMessage response = await _client.GetAsync
                (requestUri: $"api/User/confirm_registration/{userEmailHash + randomWord}");
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ConfirmChangePassword_Test()
        {
            string userEmail = "av-shvesov2015@yandex.ru";
            string userPassword = "test";
            string userEmailHash = _hashService.GetHash(userEmail);
            string passwordHash = _hashService.GetHash(userPassword);
            string randomWord = "hhh";
            Configuration.randomWord = randomWord;
            Configuration.user = new User { Email = userEmail, PasswordHash = "test" };

            HttpResponseMessage response = await _client.GetAsync
                (requestUri: $"api/User/confirm_change_password/{userEmailHash}/{passwordHash + randomWord}");
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/registration", false)]
        [InlineData("/login", true)]
        public async Task Registration_Login_Test(string url, bool hypothesis)
        {
            // Arrange
            var bodyContent = new AuthenticateUserCommandDto { Email = defaultUserEmail, Password = defaultUserPassword };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"api/User{url}", httpContent);            
            response.EnsureSuccessStatusCode();
            string resultContent = await response.Content.ReadAsStringAsync();
            UserAuthenticationResultDto result = JsonConvert.DeserializeObject<UserAuthenticationResultDto>(resultContent);

            // Assert
            Assert.Equal(hypothesis, result.IsSuccess);
        }

        [Fact]
        public async Task CheckToChangePassword_Test()
        {
            string userEmail = "av-shvesov2015@yandex.ru";
            string userPassword = "test";
            string newUserPassword = "testt";

            UserPasswordsDto bodyContent = new UserPasswordsDto { LastPassword = userPassword, NewPassword = newUserPassword };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            User user = new User { Email = userEmail, PasswordHash = _hashService.GetHash("test") };
            Configuration.user = user;

            HttpResponseMessage response = await _client.PostAsync
                ($"api/User/change_password", httpContent);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

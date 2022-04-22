using Application;
using Application.Services.HashService;
using Domain.Entities;
using HodoCloudAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace HodoCloudAPI.IntegrationTests
{
    public class UserControllerTests: IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly IHashService _hashService;

        public UserControllerTests(TestingWebAppFactory<Startup> factory)
        {
            _hashService = new HashService();
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:5001/");
        }

        [Fact]
        public async Task ConfirmRegistration_Test()
        {
            string userEmail = "av-shvesov2015@yandex.ru";
            string userEmailHash = _hashService.GetHash(userEmail);
            string randomWord = "hhh";
            Configuration.randomWord = randomWord;
            Configuration.user = new User { Email = userEmail, PasswordHash = "test" };

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
            response.EnsureSuccessStatusCode(); // из базы ничего не приходит поэтому ошибка, понять принцип работы InMemoryDatabase

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/registration")]
        [InlineData("/login")]
        public async Task Registration_Login_Test(string url)
        {
            string userEmail = "av-shvesov2015@yandex.ru";
            string userPassword = "test";
            var bodyContent = new AuthenticateUserCommandDto { Email = userEmail, Password = userPassword };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"api/User{url}", httpContent);            
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CheckToChangePassword_Test()
        {
            string userPassword = "test";
            string newUserPassword = "testt";
            UserPasswordsDto bodyContent = new UserPasswordsDto { LastPassword = userPassword, NewPassword = newUserPassword };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync
                ($"api/User/change_password", httpContent);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

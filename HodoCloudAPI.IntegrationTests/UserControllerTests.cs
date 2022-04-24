using Application;
using Application.Services.HashService;
using Domain.Entities;
using HodoCloudAPI.Dtos;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
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
            // Arrange
            string hypothesis = "Successfuly registration";
            string userEmailHash = _hashService.GetHash(defaultUserEmail);
            string randomWord = "hhh";
            User currentUser = new User { Email = defaultUserEmail, PasswordHash = defaultUserPassword };
            Configuration.randomWord = randomWord;
            Configuration.user = currentUser;
            var _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<AppDBContext>(options => options.UseInMemoryDatabase("MemoryDatabase"));
                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AppDBContext>();
                        db.Database.EnsureCreated();
                        db.Users.Remove(currentUser);
                        db.SaveChanges();
                    }
                });
            }).CreateClient();

            // Act
            HttpResponseMessage response = await _client.GetAsync
                (requestUri: $"api/User/confirm_registration/{userEmailHash + randomWord}");
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(result, hypothesis);
        }

        [Theory]
        [InlineData(defaultUserEmail, "Password is changed")]
        [InlineData("", "Password has not been changed")]
        public async Task ConfirmChangePassword_Test(string userEmail, string hypothesis)
        {
            // Arrange
            string userEmailHash = _hashService.GetHash(userEmail);
            string passwordHash = _hashService.GetHash(defaultUserPassword);
            string randomWord = "hhh";
            User currentUser = new User { Email = userEmail, PasswordHash = "test" };
            Configuration.randomWord = randomWord;
            Configuration.user = currentUser;
            Configuration.userPasswordHash = _hashService.GetHash(currentUser.PasswordHash);

            //Act
            HttpResponseMessage response = await _client.GetAsync
                (requestUri: $"api/User/confirm_change_password/{userEmailHash}/{passwordHash + randomWord}");
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(hypothesis, result);
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

        [Theory]
        [InlineData(true, defaultUserPassword)]
        [InlineData(false, "")]
        public async Task CheckToChangePassword_Test(bool hypothesis, string userPassword)
        {
            var _client = _factory.WithWebHostBuilder(builder =>
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

            string newUserPassword = "test";
            UserPasswordsDto bodyContent = new UserPasswordsDto { LastPassword = userPassword, NewPassword = newUserPassword };
            string json = JsonConvert.SerializeObject(bodyContent);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync
                ($"api/User/change_password", httpContent);
            response.EnsureSuccessStatusCode();
            string jsonResult = await response.Content.ReadAsStringAsync();
            UserAuthenticationResultDto result = JsonConvert.DeserializeObject<UserAuthenticationResultDto>(jsonResult);

            Assert.Equal(hypothesis, result.IsSuccess);
        }

        [Fact]
        public async Task IsUserAuthorized_Test()
        {
            string hypothesis = defaultUserEmail;

            var _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });               
                });
            }).CreateClient();

            HttpResponseMessage response = await _client.GetAsync
                ($"api/User/is_authorized");
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();

            Assert.Equal(hypothesis, result);
        }

        [Fact]
        public async Task Logout_Test()
        {
            var _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                });
            }).CreateClient();

            HttpResponseMessage response = await _client.PostAsync
                ($"api/User/logout", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(ClaimTypes.Name, defaultUserEmail) };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }
    }
}

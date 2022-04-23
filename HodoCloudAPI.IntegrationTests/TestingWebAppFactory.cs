using Application.Services.HashService;
using Domain.Entities;
using HodoCloudAPI;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;

public class TestingWebAppFactory<TStartup> : WebApplicationFactory<Startup> where TStartup: class
{
    private readonly IHashService _hashService = new HashService();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AppDBContext>));

            services.Remove(descriptor);

            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDBContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<TestingWebAppFactory<TStartup>>>();

                db.Database.EnsureCreated();

                try
                {
                    db.Users.AddRange(new User { Email = "a@yandex.ru", PasswordHash = _hashService.GetHash("test") });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                        "database with test messages. Error: {Message}", ex.Message);
                }
            }
        });
    }
}
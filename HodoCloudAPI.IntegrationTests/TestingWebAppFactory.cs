using HodoCloudAPI;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

public class TestingWebAppFactory<TStartup> : WebApplicationFactory<Startup> where TStartup: class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(DbContextOptions<AppDBContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryEmployeeTest");
                options.UseInternalServiceProvider(serviceProvider);
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<AppDBContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            }
        });
    }
}
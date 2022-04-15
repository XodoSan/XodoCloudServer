using Application;
using Application.Services.AuthService;
using Application.Services.EmailSenderService;
using Application.Services.FileService;
using Application.Services.UserService;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Loader;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace HodoCloudAPI
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup(IConfiguration configuration)
        {
            Config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IFileLoader, FileLoader>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IAuthService, AuthService>();

            IConfiguration config = GetConfig();
            string connectionString = config.GetConnectionString("XodoCloudDB");
            Configuration.emailSender = config.GetConnectionString("EmailSender");
            Configuration.userPassword = config.GetConnectionString("EmailPassword");

            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("Infrastructure")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });

            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IConfiguration GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }
}

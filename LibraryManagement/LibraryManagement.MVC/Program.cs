using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using LibraryManagement.MVC.Interface.API.Books;
using LibraryManagement.MVC.Interface.API.Dashboard;
using LibraryManagement.MVC.Services.API.Books;
using LibraryManagement.MVC.Services.API.Dashboard;


namespace LibraryManagement.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(
                 CookieAuthenticationDefaults.AuthenticationScheme)
                     .AddCookie(options =>
                     {
                         options.LoginPath = "/Account/Login";
                         options.AccessDeniedPath = "/Account/AccessDenied";
                         options.ExpireTimeSpan = TimeSpan.FromHours(2);
                     });

            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress =
                       new Uri("https://localhost:7229/");
            });

            builder.Services.AddAuthorization();
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

            builder.Services.AddHttpClient<IBookApiClient, BookApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<IStaffDashboardApiClient, StaffDashboardApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<LibraryApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

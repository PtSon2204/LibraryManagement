using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.Services;

namespace LibraryManagement.MVC.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var apiUrl = configuration["ApiSettings:BaseUrl"];

            services.AddHttpClient<IAccountService, AccountService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<IUserProfileService, UserProfileService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            return services;
        }
    }
}
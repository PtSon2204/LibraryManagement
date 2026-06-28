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

            // Đăng ký JwtAuthorizationHandler
            services.AddTransient<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IAccountService, AccountService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IUserProfileService, UserProfileService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IPublisherService, PublisherService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IBookService, BookService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IDashboardService, DashboardService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<IBookCopyService, BookCopyService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            services.AddHttpClient<ILoanService, LoanService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            })
            .AddHttpMessageHandler<LibraryManagement.MVC.Handlers.JwtAuthorizationHandler>();

            return services;
        }
    }
}
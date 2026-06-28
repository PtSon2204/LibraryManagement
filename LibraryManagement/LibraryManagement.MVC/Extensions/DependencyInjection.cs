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
            var apiUrl = configuration["ApiSettings:BaseUrl"]
                         ?? throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

            services.AddHttpClient<IAccountService, AccountService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<IBookService, BookService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<IUserProfileService, UserProfileService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<IStaffDashboardService, StaffDashboardService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<ILoanService, LoanService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            services.AddHttpClient<IPublisherService, PublisherService>(x =>
            {
                x.BaseAddress = new Uri(apiUrl);
            });

            return services;
        }
    }
}

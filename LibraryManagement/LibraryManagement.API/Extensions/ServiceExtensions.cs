using LibraryManagement.Business.Interfaces;
using LibraryManagement.Business.Services;
using LibraryManagement.Business.Validators.AuthValidators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace LibraryManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Đăng kí serivce
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookQueryService,  BookQueryService>();
            services.AddScoped<IStaffDashboardService,  StaffDashboardService>();
            services.AddSingleton<ILibraryPolicyService, LibraryPolicyService>();

            //Đăng kí fluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

            return services;
        }
    }
}

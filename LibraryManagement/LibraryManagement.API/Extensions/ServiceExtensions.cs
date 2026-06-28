using LibraryManagement.Business.Interfaces;
using LibraryManagement.Business.Services;
using LibraryManagement.Business.Validators.AuthValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagement.Business.DTOs.EmailDTOs;

namespace LibraryManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Đăng kí serivce
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddScoped<IStaffDashboardService, StaffDashboardService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IBookCopyService, BookCopyService>();
            services.AddScoped<ILoanService, LoanService>();

            //Đăng kí fluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

            // cấu hình email
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            //cấu hình authen
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                           .AddJwtBearer(options =>
                           {
                               options.TokenValidationParameters =
                                   new TokenValidationParameters
                                   {
                                       ValidateIssuer = true,
                                       ValidateAudience = true,
                                       ValidateLifetime = true,
                                       ValidateIssuerSigningKey = true,
                          
                                       ValidIssuer = configuration["Jwt:Issuer"],
                                       ValidAudience = configuration["Jwt:Audience"],
                          
                                       IssuerSigningKey =
                                           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                               configuration["Jwt:Key"]
                                               ?? throw new InvalidOperationException("Jwt:Key is not configured.")))
                                   };
                           });

            services.AddAuthorization();

         


            return services;
        }
    }
}

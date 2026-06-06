using LibraryManagement.Business.Interfaces;
using LibraryManagement.Business.Services;
using LibraryManagement.Business.Validators.AuthValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Đăng kí serivce
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            //Đăng kí fluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

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
                                           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                                   };
                           });

            services.AddAuthorization();


            return services;
        }
    }
}

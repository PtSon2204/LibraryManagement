namespace LibraryManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Đăng kí serivce


            //Đăng kí fluentValidation
            //services.AddFluentValidationAutoValidation();
            //services.AddValidatorsFromAssemblyContaining<CreateRoleDtoValidator>();

            return services;
        }
    }
}

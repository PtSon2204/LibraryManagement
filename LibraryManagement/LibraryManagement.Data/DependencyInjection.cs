using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Repositories;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReaderRepository, ReaderRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            return services;
        }
    }
}

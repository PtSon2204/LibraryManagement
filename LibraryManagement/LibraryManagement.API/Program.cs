//using LibraryManagement.API.Extensions;
using LibraryManagement.API.Middleware;
using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Data;
using LibraryManagement.Models.Context;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace LibraryManagement.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));


            //Đăng kí repository
            builder.Services.AddData();

            //Đăng kí service
           // builder.Services.AddCustomServices(builder.Configuration);

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers()
                .AddOData(options =>
                    options.Select()
                            .Filter()
                            .OrderBy()
                            .Expand()
                            .Count()
                            .SetMaxTop(100)
                            .AddRouteComponents("odata", GetEdmModel()));

            var app = builder.Build();

            //SeedData
            using (var scope = app.Services.CreateScope())
            {
                await SeedData.Initialize(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<BookOdataDto>("Books")
               .EntityType
               .HasKey(b => b.BookId);

            return builder.GetEdmModel();
        }
    }
}

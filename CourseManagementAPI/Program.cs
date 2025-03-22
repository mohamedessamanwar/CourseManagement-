using BusinessAccessLayer;
using BusinessAccessLayer.DTOS;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Api.Middlewares;
using Serilog;
using Serilog.Events;
namespace CourseManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.DataAccessLayerServicesRegister(builder.Configuration);
            builder.Services.BusinessAccessLayerRegister(builder.Configuration);
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    Dictionary<string, List<string>> errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        );

                    return new BadRequestObjectResult(new Response<Dictionary<string, List<string>>>(errors,
                        false, "Failed"));

                };
            });
            builder.Host.UseSerilog((context, config) =>
            {
                config.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
                config.WriteTo.Console();
                config.WriteTo.File("Logs/CompanyLogs-20241220.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
            });
            builder.Services.AddTransient<ErrorHandleMiddleware>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ErrorHandleMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}

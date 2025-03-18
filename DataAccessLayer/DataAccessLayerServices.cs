using DataAccessLayer.Data;
using DataAccessLayer.Repositories.CourseRepo;
using DataAccessLayer.Repositories.CourseTrainerRepo;
using DataAccessLayer.Repositories.PaymentRepo;
using DataAccessLayer.Repositories.TrainerRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer
{
    public static class DataAccessLayerServices
    {
        public static IServiceCollection DataAccessLayerServicesRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(optionsAction => { optionsAction.UseSqlServer(configuration.GetConnectionString("DefaultConnection")); });
            services.AddScoped<ITrainerRepo, TrainerRepo>();
            services.AddScoped<ICourseRepo, CourseRepo>();
            services.AddScoped<IPaymentRepo, PaymentRepo>();
            services.AddScoped<ICourseTrainerRepo, CourseTrainerRepo>();
            return services;
        }

    }
}

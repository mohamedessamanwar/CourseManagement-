using DataAccessLayer.Data;
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
            return services;
        }

    }
}

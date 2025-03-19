using BusinessAccessLayer.DTOS.AuthDtos;
using BusinessAccessLayer.Services.AuthService;
using BusinessAccessLayer.Services.CourseService;
using BusinessAccessLayer.Services.Email;
using BusinessAccessLayer.Services.TrainerService;
using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace BusinessAccessLayer
{
    public static class BusinessAccessLayerServices
    {

        public static IServiceCollection BusinessAccessLayerRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMailingService, MailingService>();
            services.Configure<MailSetting>(configuration.GetSection("MailSetting"));
            services.Configure<JWT>(configuration.GetSection("JWT"));
            //services.AddSingleton<ICacheService, CacheService>();
            services.AddIdentity<User, IdentityRole>()
                         .AddEntityFrameworkStores<ApplicationDbContext>()
                         .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(o =>
             {
                 o.RequireHttpsMetadata = false;
                 o.SaveToken = false;
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidIssuer = configuration["JWT:Issuer"],
                     ValidAudience = configuration["JWT:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                 };
             });
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<ICourseService, CourseService>();


            return services;

        }
    }
}
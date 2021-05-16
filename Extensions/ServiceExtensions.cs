using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Extension Method to Add Configuration Services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns>A reference to this object after the operation has completed</returns>
        public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SQLite"));
            });
            services.AddIdentityCore<EntityUser>(setupAction =>
            {
                setupAction.User.RequireUniqueEmail = true;
                setupAction.Password.RequireNonAlphanumeric = false;
                setupAction.Password.RequireDigit = false;
                setupAction.Password.RequiredLength = 4;
                setupAction.Password.RequireLowercase = false;
                setupAction.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager<SignInManager<EntityUser>>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_SECRET"])),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            return services;
        }
    }
}
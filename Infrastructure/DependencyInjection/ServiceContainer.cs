using Application.Extention.Identity;
using Application.Interface.ActivityTracker;
using Application.Interface.Identity;
using Infrastructure.DataAccess;
using Infrastructure.Reposito;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

            // 🔹 Identity
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // 🔹 Authorization (Policies)
            services.AddAuthorizationBuilder()
                .AddPolicy("AdministrationPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin", "Manager");
                })
                .AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("User");
                });

            // 🔹 Services
            services.AddScoped<IAccount, Account>();
            services.AddScoped<IActivityTracker, ActivityTrackerRepository>();

            return services;
        }
    }
}
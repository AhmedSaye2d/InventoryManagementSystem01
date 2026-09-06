using System.Text;
using Application.Extention.Identity;
using Application.Interface.ActivityTracker;
using Application.Interface.Identity;
using Infrastructure.DataAccess;
using Infrastructure.Reposito;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = configuration["Jwt:Key"] ?? "DefaultSuperSecretKey1234567890123456!";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "InventoryApi",
                    ValidAudience = configuration["Jwt:Audience"] ?? "InventoryClient",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            // 🔹 Identity
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // 🔹 Authorization (Policies)
            services.AddAuthorizationBuilder()
                .AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                })
                .AddPolicy("ManagerPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin", "Manager");
                })
                .AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                })
                .AddPolicy("CreatePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(ctx => ctx.User.HasClaim("create", "true") || ctx.User.IsInRole("Admin") || ctx.User.IsInRole("Manager"));
                })
                .AddPolicy("UpdatePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(ctx => ctx.User.HasClaim("update", "true") || ctx.User.IsInRole("Admin") || ctx.User.IsInRole("Manager"));
                })
                .AddPolicy("DeletePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(ctx => ctx.User.HasClaim("delete", "true") || ctx.User.IsInRole("Admin"));
                })
                .AddPolicy("ReadPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                })
                .AddPolicy("ManageUserPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(ctx => ctx.User.HasClaim("MangerUser", "true") || ctx.User.IsInRole("Admin"));
                });

            // 🔹 Repositories & Unit of Work
            services.AddScoped(typeof(Application.Interfaces.Repositories.IGenericRepository<>), typeof(Infrastructure.Repositories.GenericRepository<>));
            services.AddScoped<Application.Interfaces.Repositories.IProductRepository, Infrastructure.Repositories.ProductRepository>();
            services.AddScoped<Application.Interfaces.Repositories.ICategoryRepository, Infrastructure.Repositories.CategoryRepository>();
            services.AddScoped<Application.Interfaces.Repositories.ILocationRepository, Infrastructure.Repositories.LocationRepository>();
            services.AddScoped<Application.Interfaces.Repositories.IOrderRepository, Infrastructure.Repositories.OrderRepository>();
            services.AddScoped<Application.Interfaces.Repositories.IInventoryMovementRepository, Infrastructure.Repositories.InventoryMovementRepository>();
            services.AddScoped<Application.Interfaces.Repositories.IUnitOfWork, Infrastructure.UnitOfWork.UnitOfWork>();

            // 🔹 Services
            services.AddScoped<IAccount, Account>();
            services.AddScoped<IActivityTracker, ActivityTrackerRepository>();

            return services;
        }
    }
}
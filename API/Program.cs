using Infrastructure.DependencyInjection;
using FluentValidation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid JWT token below."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

// MediatR: scan Application (commands/queries) + Infrastructure (handlers)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Application.Services.Product.Commands.Products.CreateProductCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Infrastructure.Reposito.Product.Handlers.Products.CreateProductHandler).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<Application.DTO.Request.Product.Validators.AddLocationRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Application.DTO.Request.Inventory.Validators.InventoryMovementFilterRequestDtoValidator>();
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Infrastructure.Behaviors.ValidationBehavior<,>));
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
var app = builder.Build();

// Seed data (Idempotent) - runs on startup, respects Configuration for credentials
using (var scope = app.Services.CreateScope())
{
    try
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var enableSeed = config.GetValue<bool?>("SeedData:Enabled") ?? true;
        if (enableSeed)
        {
            await Infrastructure.DataAccess.Seed.AppDbSeeder.SeedAsync(app.Services);
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seeding failed");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Infrastructure.Middleware.RequestLoggingMiddleware>();
app.UseMiddleware<Infrastructure.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

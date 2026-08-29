using Infrastructure.DependencyInjection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Application.DTO.Request.Product.Validators.AddLocationRequestDtoValidator>();
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Infrastructure.Behaviors.ValidationBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Infrastructure.Middleware.RequestLoggingMiddleware>();
app.UseMiddleware<Infrastructure.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

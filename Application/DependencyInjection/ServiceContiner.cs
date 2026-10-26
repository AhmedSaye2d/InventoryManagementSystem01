using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ServiceContiner
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            return services;
        }
    }
}

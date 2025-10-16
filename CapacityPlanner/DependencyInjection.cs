using Microsoft.Extensions.DependencyInjection;
using Persist.CapacityPlanner;
using Services.CapacityPlanner;

namespace CapacityPlanner;

public static class DependencyInjection
{
    public static IServiceCollection AddCapacityPlannerServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCapacityPlannerApplication();
        return services;
    }
}

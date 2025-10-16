using Microsoft.Extensions.DependencyInjection;

namespace Services.CapacityPlanner;

public static class DependencyInjection
{
    public static IServiceCollection AddCapacityPlannerApplication(this IServiceCollection services)
    {
        // Register application-layer services here
        services.AddScoped<IScenarioService, ScenarioService>();
        return services;
    }
}

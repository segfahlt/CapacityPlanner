using Common.CapacityPlanner;

using Microsoft.Extensions.DependencyInjection;

using Persist.CapacityPlanner;

using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public static class DependencyInjection
{
    public static IServiceCollection AddCapacityPlannerApplication(this IServiceCollection services)
    {
		// Register application-layer services here
		services.AddCommonCapacityPlanner();
		services.AddScoped<IScenarioService, ScenarioService>();


		return services;
    }
}

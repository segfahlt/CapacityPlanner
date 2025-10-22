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
        services.AddScoped<Abstraction.IRoleService, RoleService>();
        services.AddScoped<Abstraction.ISkillService, SkillService>();
        services.AddScoped<Abstraction.IPersonService, PersonService>();
        services.AddScoped<Abstraction.IPersonSkillService, PersonSkillService>();
        services.AddScoped<Abstraction.IPlatformService, PlatformService>();
        services.AddScoped<Abstraction.IModuleService, ModuleService>();
        services.AddScoped<Abstraction.IImplementationTemplateService, ImplementationTemplateService>();


		return services;
    }
}

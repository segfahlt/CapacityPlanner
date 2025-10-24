using Common.CapacityPlanner;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persist.CapacityPlanner.DbModel;

namespace Persist.CapacityPlanner;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CapacityPlanner");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:CapacityPlanner is not configured.");

        services.AddDbContext<CapacityPlannerContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            });
        });

        services.AddCommonCapacityPlanner();
        
		return services;
    }
}

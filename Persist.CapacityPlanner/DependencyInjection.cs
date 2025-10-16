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

        // Optional: factory for background operations or explicit context creation
        services.AddDbContextFactory<CapacityPlannerContext>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
        });

        return services;
    }
}

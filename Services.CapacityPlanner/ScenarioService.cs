using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;

using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public sealed class ScenarioService : IScenarioService
{
    private readonly CapacityPlannerContext _db;

    public ScenarioService(CapacityPlannerContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateScenarioAsync(string name, string? description, Guid? createdBy, CancellationToken ct = default)
    {
        var entity = new Persist.CapacityPlanner.DbModel.Entities.Scenario
        {
            ScenarioId = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedBy = createdBy,
            IsActive = false,
            CreatedOn = DateTime.UtcNow
        };
        _db.Scenario.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ScenarioId;
    }
}

namespace Services.CapacityPlanner.Abstraction;

public interface IScenarioService
{
    Task<Guid> CreateScenarioAsync(string name, string? description, Guid? createdBy, CancellationToken ct = default);
}

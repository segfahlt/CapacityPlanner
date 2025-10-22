using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Abstraction;

public interface IModuleService
{
    Task<List<Module>> ListAsync(CancellationToken ct = default);
    Task<Module?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(Guid platformId, string name, string? description, string? status, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, string name, string? description, string? status, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

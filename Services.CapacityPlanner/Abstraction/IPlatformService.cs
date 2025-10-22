using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Abstraction;

public interface IPlatformService
{
    Task<List<Platform>> ListAsync(CancellationToken ct = default);
    Task<Platform?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(string name, string? description, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, string name, string? description, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

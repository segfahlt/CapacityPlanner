using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Abstraction;

public interface IRoleService
{
    Task<List<Role>> ListAsync(CancellationToken ct = default);
    Task<Role?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(string name, decimal? defaultUtilizationTarget, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, string name, decimal? defaultUtilizationTarget, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IRoleService
{
    Task<List<RoleDto>> ListAsync(CancellationToken ct = default);
    Task<RoleDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(RoleDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(RoleDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IModuleService
{
    Task<List<ModuleDto>> ListAsync(CancellationToken ct = default);
    Task<ModuleDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(ModuleDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(ModuleDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

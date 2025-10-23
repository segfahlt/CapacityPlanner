using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IPlatformService
{
    Task<List<PlatformDto>> ListAsync(CancellationToken ct = default);
    Task<PlatformDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(PlatformDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(PlatformDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

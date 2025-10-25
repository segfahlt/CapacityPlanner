using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IImplementationService
{
 Task<List<ImplementationDto>> ListAsync(Guid? moduleId = null, Guid? platformId = null, CancellationToken ct = default);
 Task<ImplementationDto?> GetAsync(Guid id, CancellationToken ct = default);
 Task<Guid> CreateAsync(ImplementationDto dto, CancellationToken ct = default);
 Task<bool> UpdateAsync(ImplementationDto dto, CancellationToken ct = default);
 Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

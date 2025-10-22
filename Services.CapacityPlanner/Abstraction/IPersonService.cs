using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IPersonService
{
    Task<List<PersonDto>> ListAsync(CancellationToken ct = default);
    Task<PersonDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(PersonDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(PersonDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

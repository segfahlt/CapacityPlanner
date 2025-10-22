using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface IPersonSkillService
{
    Task<List<PersonSkillDto>> ListAsync(CancellationToken ct = default);
    Task<PersonSkillDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(PersonSkillDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(PersonSkillDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

using Common.CapacityPlanner.Dto;

namespace Services.CapacityPlanner.Abstraction;

public interface ISkillService
{
    Task<List<SkillDto>> ListAsync(CancellationToken ct = default);
    Task<SkillDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(SkillDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(SkillDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

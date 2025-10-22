using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class SkillService : ISkillService
{
    private readonly CapacityPlannerContext _db;
    public SkillService(CapacityPlannerContext db) => _db = db;

    public async Task<List<SkillDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.Skill.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct)).Select(s => s.ToDto()).ToList();

    public async Task<SkillDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Skill.AsNoTracking().FirstOrDefaultAsync(s => s.SkillId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(SkillDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var exists = await _db.Skill.AnyAsync(s => s.Name == dto.Name, ct);
        if (exists) throw new InvalidOperationException("Skill name must be unique");
        var skill = new Skill { SkillId = dto.SkillId == Guid.Empty ? Guid.NewGuid() : dto.SkillId, Name = dto.Name, Description = dto.Description };
        _db.Skill.Add(skill);
        await _db.SaveChangesAsync(ct);
        return skill.SkillId;
    }

    public async Task<bool> UpdateAsync(SkillDto dto, CancellationToken ct = default)
    {
        var id = dto.SkillId;
        var skill = await _db.Skill.FirstOrDefaultAsync(s => s.SkillId == id, ct);
        if (skill is null) return false;
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name required", nameof(dto.Name));
        var taken = await _db.Skill.AnyAsync(s => s.Name == dto.Name && s.SkillId != id, ct);
        if (taken) throw new InvalidOperationException("Skill name must be unique");
        skill.Name = dto.Name;
        skill.Description = dto.Description;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var skill = await _db.Skill.FirstOrDefaultAsync(s => s.SkillId == id, ct);
        if (skill is null) return false;
        _db.Skill.Remove(skill);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

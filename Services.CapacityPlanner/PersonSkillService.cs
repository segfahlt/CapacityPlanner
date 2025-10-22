using Common.CapacityPlanner.Dto;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;
using Services.CapacityPlanner.Mapping;

namespace Services.CapacityPlanner;

public sealed class PersonSkillService : IPersonSkillService
{
    private readonly CapacityPlannerContext _db;
    public PersonSkillService(CapacityPlannerContext db) => _db = db;

    public async Task<List<PersonSkillDto>> ListAsync(CancellationToken ct = default) =>
        (await _db.PersonSkill.AsNoTracking().ToListAsync(ct)).Select(ps => ps.ToDto()).ToList();

    public async Task<PersonSkillDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.PersonSkill.AsNoTracking().FirstOrDefaultAsync(ps => ps.PersonSkillId == id, ct);
        return e?.ToDto();
    }

    public async Task<Guid> CreateAsync(PersonSkillDto dto, CancellationToken ct = default)
    {
        // Validate FKs
        var personExists = await _db.Person.AsNoTracking().AnyAsync(p => p.PersonId == dto.PersonId, ct);
        if (!personExists) throw new InvalidOperationException("Person not found");
        var skillExists = await _db.Skill.AsNoTracking().AnyAsync(s => s.SkillId == dto.SkillId, ct);
        if (!skillExists) throw new InvalidOperationException("Skill not found");

        // Enforce uniqueness (PersonID, SkillID)
        var taken = await _db.PersonSkill.AsNoTracking().AnyAsync(ps => ps.PersonId == dto.PersonId && ps.SkillId == dto.SkillId, ct);
        if (taken) throw new InvalidOperationException("Person already has this skill");

        var entity = dto.ToEntity();
        if (entity.PersonSkillId == Guid.Empty) entity.PersonSkillId = Guid.NewGuid();
        _db.PersonSkill.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PersonSkillId;
    }

    public async Task<bool> UpdateAsync(PersonSkillDto dto, CancellationToken ct = default)
    {
        var id = dto.PersonSkillId;
        var entity = await _db.PersonSkill.FirstOrDefaultAsync(ps => ps.PersonSkillId == id, ct);
        if (entity is null) return false;
        entity.ProficiencyLevel = dto.ProficiencyLevel;
        entity.Notes = dto.Notes;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.PersonSkill.FirstOrDefaultAsync(ps => ps.PersonSkillId == id, ct);
        if (entity is null) return false;
        _db.PersonSkill.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;
using Persist.CapacityPlanner.DbModel.Entities;
using Services.CapacityPlanner.Abstraction;

namespace Services.CapacityPlanner;

public sealed class ImplementationTemplateService : IImplementationTemplateService
{
    private readonly CapacityPlannerContext _db;
    public ImplementationTemplateService(CapacityPlannerContext db) => _db = db;

    public async Task<Guid> CreateTemplateAsync(string name, string? description, string? category, bool isActive, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        var taken = await _db.ImplementationTemplate.AsNoTracking().AnyAsync(t => t.Name == name, ct);
        if (taken) throw new InvalidOperationException("Template name must be unique");
        var entity = new ImplementationTemplate { TemplateId = Guid.NewGuid(), Name = name, Description = description, Category = category, IsActive = isActive };
        _db.ImplementationTemplate.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.TemplateId;
    }

    public async Task<Guid> AddPhaseAsync(Guid templateId, string phaseName, int sequence, int? defaultDurationWeeks, int? defaultStartOffsetWeeks, string? defaultStatus, string? notes, CancellationToken ct = default)
    {
        var templateExists = await _db.ImplementationTemplate.AsNoTracking().AnyAsync(t => t.TemplateId == templateId, ct);
        if (!templateExists) throw new InvalidOperationException("Template not found");
        if (string.IsNullOrWhiteSpace(phaseName)) throw new ArgumentException("PhaseName required", nameof(phaseName));
        var entity = new ImplementationTemplatePhase
        {
            PhaseId = Guid.NewGuid(),
            TemplateId = templateId,
            PhaseName = phaseName,
            Sequence = sequence,
            DefaultDurationWeeks = defaultDurationWeeks,
            DefaultStartOffsetWeeks = defaultStartOffsetWeeks,
            DefaultStatus = defaultStatus,
            Notes = notes
        };
        _db.ImplementationTemplatePhase.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PhaseId;
    }

    public async Task<Guid> AddPhaseDemandAsync(Guid phaseId, Guid roleId, decimal? defaultFte, string? notes, CancellationToken ct = default)
    {
        var phaseExists = await _db.ImplementationTemplatePhase.AsNoTracking().AnyAsync(p => p.PhaseId == phaseId, ct);
        if (!phaseExists) throw new InvalidOperationException("Phase not found");
        var roleExists = await _db.Role.AsNoTracking().AnyAsync(r => r.RoleId == roleId, ct);
        if (!roleExists) throw new InvalidOperationException("Role not found");
        var taken = await _db.ImplementationTemplatePhaseDemand.AsNoTracking().AnyAsync(d => d.PhaseId == phaseId && d.RoleId == roleId, ct);
        if (taken) throw new InvalidOperationException("Demand for role already exists in this phase");
        var entity = new ImplementationTemplatePhaseDemand
        {
            PhaseDemandId = Guid.NewGuid(),
            PhaseId = phaseId,
            RoleId = roleId,
            DefaultFte = defaultFte,
            Notes = notes
        };
        _db.ImplementationTemplatePhaseDemand.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PhaseDemandId;
    }

    public async Task<Guid> AddPhaseDemandSkillAsync(Guid phaseDemandId, Guid skillId, string? priorityLevel, string? notes, CancellationToken ct = default)
    {
        var demandExists = await _db.ImplementationTemplatePhaseDemand.AsNoTracking().AnyAsync(d => d.PhaseDemandId == phaseDemandId, ct);
        if (!demandExists) throw new InvalidOperationException("PhaseDemand not found");
        var skillExists = await _db.Skill.AsNoTracking().AnyAsync(s => s.SkillId == skillId, ct);
        if (!skillExists) throw new InvalidOperationException("Skill not found");
        var taken = await _db.ImplementationTemplatePhaseDemandSkill.AsNoTracking().AnyAsync(s => s.PhaseDemandId == phaseDemandId && s.SkillId == skillId, ct);
        if (taken) throw new InvalidOperationException("Skill already exists on this phase demand");
        var entity = new ImplementationTemplatePhaseDemandSkill
        {
            PhaseDemandSkillId = Guid.NewGuid(),
            PhaseDemandId = phaseDemandId,
            SkillId = skillId,
            PriorityLevel = priorityLevel,
            Notes = notes
        };
        _db.ImplementationTemplatePhaseDemandSkill.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.PhaseDemandSkillId;
    }

    public async Task<ImplementationTemplate?> GetTemplateAsync(Guid templateId, bool includeGraph = false, CancellationToken ct = default)
    {
        IQueryable<ImplementationTemplate> query = _db.ImplementationTemplate.AsNoTracking();
        if (includeGraph)
        {
            query = query
                .Include(t => t.ImplementationTemplatePhase)
                    .ThenInclude(p => p.ImplementationTemplatePhaseDemand)
                        .ThenInclude(d => d.ImplementationTemplatePhaseDemandSkill);
        }
        return await query.FirstOrDefaultAsync(t => t.TemplateId == templateId, ct);
    }

    public async Task<Guid> CreateTemplateAsync(Contracts.ImplementationTemplateCreateRequest request, CancellationToken ct = default)
    {
        // Validate top-level
        var templateId = await CreateTemplateAsync(request.Name, request.Description, request.Category, request.IsActive, ct);

        // Phases
        foreach (var p in request.Phases.OrderBy(x => x.Sequence))
        {
            var phaseId = await AddPhaseAsync(templateId, p.PhaseName, p.Sequence, p.DefaultDurationWeeks, p.DefaultStartOffsetWeeks, p.DefaultStatus, p.Notes, ct);
            foreach (var d in p.Demands)
            {
                var demandId = await AddPhaseDemandAsync(phaseId, d.RoleId, d.DefaultFte, d.Notes, ct);
                foreach (var s in d.Skills)
                {
                    await AddPhaseDemandSkillAsync(demandId, s.SkillId, s.PriorityLevel, s.Notes, ct);
                }
            }
        }
        return templateId;
    }
}

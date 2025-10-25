using Common.CapacityPlanner.Dto;
using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Mapping;

public static partial class DtoMappers
{
    // Role
    public static RoleDto ToDto(this Role e) => new() { RoleId = e.RoleId, Name = e.Name, DefaultUtilizationTarget = e.DefaultUtilizationTarget };
    public static Role ToEntity(this RoleDto d) => new() { RoleId = d.RoleId == Guid.Empty ? Guid.NewGuid() : d.RoleId, Name = d.Name, DefaultUtilizationTarget = d.DefaultUtilizationTarget };

    // Skill
    public static SkillDto ToDto(this Skill e) => new() { SkillId = e.SkillId, Name = e.Name, Description = e.Description };
    public static Skill ToEntity(this SkillDto d) => new() { SkillId = d.SkillId == Guid.Empty ? Guid.NewGuid() : d.SkillId, Name = d.Name, Description = d.Description };

    // Person
    public static PersonDto ToDto(this Person e) => new() { PersonId = e.PersonId, Name = e.Name, EmploymentType = e.EmploymentType, IsActive = e.IsActive, Location = e.Location };
    public static Person ToEntity(this PersonDto d) => new() { PersonId = d.PersonId == Guid.Empty ? Guid.NewGuid() : d.PersonId, Name = d.Name, EmploymentType = d.EmploymentType, IsActive = d.IsActive, Location = d.Location };

    // PersonSkill
    public static PersonSkillDto ToDto(this PersonSkill e) => new() { PersonSkillId = e.PersonSkillId, PersonId = e.PersonId, SkillId = e.SkillId, ProficiencyLevel = e.ProficiencyLevel, Notes = e.Notes };
    public static PersonSkill ToEntity(this PersonSkillDto d) => new() { PersonSkillId = d.PersonSkillId == Guid.Empty ? Guid.NewGuid() : d.PersonSkillId, PersonId = d.PersonId, SkillId = d.SkillId, ProficiencyLevel = d.ProficiencyLevel, Notes = d.Notes };

    // Platform
    public static PlatformDto ToDto(this Platform e) => new() { PlatformId = e.PlatformId, Name = e.Name, Description = e.Description };
    public static Platform ToEntity(this PlatformDto d) => new() { PlatformId = d.PlatformId == Guid.Empty ? Guid.NewGuid() : d.PlatformId, Name = d.Name, Description = d.Description };

    // Module
    public static ModuleDto ToDto(this Module e) => new() { ModuleId = e.ModuleId, PlatformId = e.PlatformId, Name = e.Name, Description = e.Description, Status = e.Status };
    public static Module ToEntity(this ModuleDto d) => new() { ModuleId = d.ModuleId == Guid.Empty ? Guid.NewGuid() : d.ModuleId, PlatformId = d.PlatformId, Name = d.Name, Description = d.Description, Status = d.Status };

    // Implementation Template (read mapping)
    public static ImplementationTemplateDto ToDto(this ImplementationTemplate e)
    {
        var dto = new ImplementationTemplateDto
        {
            TemplateId = e.TemplateId,
            Name = e.Name,
            Description = e.Description,
            Category = e.Category,
            IsActive = e.IsActive
        };
        if (e.ImplementationTemplatePhase is not null)
        {
            dto.Phases = e.ImplementationTemplatePhase
                .OrderBy(p => p.Sequence)
                .Select(p => new ImplementationTemplatePhaseDto
                {
                    PhaseId = p.PhaseId,
                    TemplateId = p.TemplateId,
                    PhaseName = p.PhaseName,
                    Sequence = p.Sequence,
                    DefaultDurationWeeks = p.DefaultDurationWeeks,
                    DefaultStartOffsetWeeks = p.DefaultStartOffsetWeeks,
                    DefaultStatus = p.DefaultStatus,
                    Notes = p.Notes,
                    Demands = p.ImplementationTemplatePhaseDemand?.Select(d => new ImplementationTemplatePhaseDemandDto
                    {
                        PhaseDemandId = d.PhaseDemandId,
                        PhaseId = d.PhaseId,
                        RoleId = d.RoleId,
                        DefaultFte = d.DefaultFte,
                        Notes = d.Notes,
                        Skills = d.ImplementationTemplatePhaseDemandSkill?.Select(s => new ImplementationTemplatePhaseDemandSkillDto
                        {
                            PhaseDemandSkillId = s.PhaseDemandSkillId,
                            PhaseDemandId = s.PhaseDemandId,
                            SkillId = s.SkillId,
                            PriorityLevel = s.PriorityLevel,
                            Notes = s.Notes
                        }).ToList() ?? new()
                    }).ToList() ?? new()
                }).ToList();
        }
        return dto;
    }
}

namespace Common.CapacityPlanner.Dto;

public sealed class ImplementationTemplateDto
{
    public Guid TemplateId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; }
    public List<ImplementationTemplatePhaseDto> Phases { get; set; } = new();
}

public sealed class ImplementationTemplatePhaseDto
{
    public Guid PhaseId { get; set; }
    public Guid TemplateId { get; set; }
    public required string PhaseName { get; set; }
    public int Sequence { get; set; }
    public int? DefaultDurationWeeks { get; set; }
    public int? DefaultStartOffsetWeeks { get; set; }
    public string? DefaultStatus { get; set; }
    public string? Notes { get; set; }
    public List<ImplementationTemplatePhaseDemandDto> Demands { get; set; } = new();
}

public sealed class ImplementationTemplatePhaseDemandDto
{
    public Guid PhaseDemandId { get; set; }
    public Guid PhaseId { get; set; }
    public Guid RoleId { get; set; }
    public decimal? DefaultFte { get; set; }
    public string? Notes { get; set; }
    public List<ImplementationTemplatePhaseDemandSkillDto> Skills { get; set; } = new();
}

public sealed class ImplementationTemplatePhaseDemandSkillDto
{
    public Guid PhaseDemandSkillId { get; set; }
    public Guid PhaseDemandId { get; set; }
    public Guid SkillId { get; set; }
    public string? PriorityLevel { get; set; }
    public string? Notes { get; set; }
}

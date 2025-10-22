namespace Services.CapacityPlanner.Contracts;

public sealed class ImplementationTemplateCreateRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Phase> Phases { get; set; } = new();

    public sealed class Phase
    {
        public required string PhaseName { get; set; }
        public int Sequence { get; set; }
        public int? DefaultDurationWeeks { get; set; }
        public int? DefaultStartOffsetWeeks { get; set; }
        public string? DefaultStatus { get; set; }
        public string? Notes { get; set; }
        public List<Demand> Demands { get; set; } = new();
    }

    public sealed class Demand
    {
        public required Guid RoleId { get; set; }
        public decimal? DefaultFte { get; set; }
        public string? Notes { get; set; }
        public List<DemandSkill> Skills { get; set; } = new();
    }

    public sealed class DemandSkill
    {
        public required Guid SkillId { get; set; }
        public string? PriorityLevel { get; set; }
        public string? Notes { get; set; }
    }
}

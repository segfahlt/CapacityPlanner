namespace Common.CapacityPlanner.Dto;

public sealed class PersonSkillDto
{
    public Guid PersonSkillId { get; set; }
    public Guid PersonId { get; set; }
    public Guid SkillId { get; set; }
    public string? ProficiencyLevel { get; set; }
    public string? Notes { get; set; }
}

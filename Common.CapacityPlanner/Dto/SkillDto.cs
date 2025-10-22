namespace Common.CapacityPlanner.Dto;

public sealed class SkillDto
{
    public Guid SkillId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

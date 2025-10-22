namespace Common.CapacityPlanner.Dto;

public sealed class PlatformDto
{
    public Guid PlatformId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

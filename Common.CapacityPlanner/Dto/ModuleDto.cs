namespace Common.CapacityPlanner.Dto;

public sealed class ModuleDto
{
    public Guid ModuleId { get; set; }
    public Guid PlatformId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}

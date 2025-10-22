namespace Common.CapacityPlanner.Dto;

public sealed class RoleDto
{
    public Guid RoleId { get; set; }
    public required string Name { get; set; }
    public decimal? DefaultUtilizationTarget { get; set; }
}

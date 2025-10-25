namespace Common.CapacityPlanner.Dto;

public sealed class ImplementationDto
{
 public Guid ImplementationId { get; set; }
 public Guid PlatformId { get; set; }
 public Guid? ModuleId { get; set; }
 public required string ClientName { get; set; }
 public string? Description { get; set; }
 public DateOnly? StartDate { get; set; }
 public DateOnly? GoLiveDate { get; set; }
 public string? Status { get; set; }
}

namespace Common.CapacityPlanner.Dto;

public sealed class PersonDto
{
    public Guid PersonId { get; set; }
    public required string Name { get; set; }
    public string? EmploymentType { get; set; }
    public bool? IsActive { get; set; }
    public string? Location { get; set; }
}

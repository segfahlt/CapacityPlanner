namespace Common.CapacityPlanner.Dto;

public sealed record LookupTypeDto(string Type, string DisplayName);

public sealed record LookupItemDto(string Key, string? Description);

using Common.CapacityPlanner.Dto;
using Persist.CapacityPlanner.DbModel.Entities;

namespace Services.CapacityPlanner.Mapping;

public static partial class DtoMappers
{
 public static ImplementationDto ToDto(this Implementation e) => new()
 {
 ImplementationId = e.ImplementationId,
 PlatformId = e.PlatformId,
 ModuleId = e.ModuleId,
 ClientName = e.ClientName,
 Description = e.Description,
 StartDate = e.StartDate,
 GoLiveDate = e.GoLiveDate,
 Status = e.Status
 };

 public static Implementation ToEntity(this ImplementationDto d) => new()
 {
 ImplementationId = d.ImplementationId == Guid.Empty ? Guid.NewGuid() : d.ImplementationId,
 PlatformId = d.PlatformId,
 ModuleId = d.ModuleId,
 ClientName = d.ClientName,
 Description = d.Description,
 StartDate = d.StartDate,
 GoLiveDate = d.GoLiveDate,
 Status = d.Status
 };
}

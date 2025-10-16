using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? DefaultUtilizationTarget { get; set; }

    public virtual ICollection<Capacity> Capacity { get; set; } = new List<Capacity>();

    public virtual ICollection<Demand> Demand { get; set; } = new List<Demand>();

    public virtual ICollection<ForecastResult> ForecastResult { get; set; } = new List<ForecastResult>();

    public virtual ICollection<ImplementationTemplatePhaseDemand> ImplementationTemplatePhaseDemand { get; set; } = new List<ImplementationTemplatePhaseDemand>();
}

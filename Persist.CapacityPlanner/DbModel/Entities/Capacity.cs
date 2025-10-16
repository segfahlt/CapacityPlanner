using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Capacity
{
    public Guid CapacityId { get; set; }

    public Guid RoleId { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public decimal AvailableFte { get; set; }

    public decimal? EffectiveUtilization { get; set; }

    public virtual Role Role { get; set; } = null!;
}

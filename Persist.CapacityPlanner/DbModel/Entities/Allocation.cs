using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Allocation
{
    public Guid AllocationId { get; set; }

    public Guid PersonId { get; set; }

    public Guid WorkstreamId { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public decimal AllocationPct { get; set; }

    public string? AllocationType { get; set; }

    public string? Source { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual Workstream Workstream { get; set; } = null!;
}

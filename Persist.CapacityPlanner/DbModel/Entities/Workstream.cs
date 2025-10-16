using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Workstream
{
    public Guid WorkstreamId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public Guid? ImplementationId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Actual> Actual { get; set; } = new List<Actual>();

    public virtual ICollection<Allocation> Allocation { get; set; } = new List<Allocation>();

    public virtual ICollection<Demand> Demand { get; set; } = new List<Demand>();

    public virtual Implementation? Implementation { get; set; }
}

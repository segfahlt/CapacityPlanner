using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Demand
{
    public Guid DemandId { get; set; }

    public Guid WorkstreamId { get; set; }

    public Guid RoleId { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public decimal RequiredFte { get; set; }

    public decimal? Confidence { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<DemandSkill> DemandSkill { get; set; } = new List<DemandSkill>();

    public virtual Role Role { get; set; } = null!;

    public virtual Workstream Workstream { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class DemandSkill
{
    public Guid DemandSkillId { get; set; }

    public Guid DemandId { get; set; }

    public Guid SkillId { get; set; }

    public string? PriorityLevel { get; set; }

    public string? Notes { get; set; }

    public virtual Demand Demand { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}

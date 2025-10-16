using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class ImplementationTemplatePhaseDemandSkill
{
    public Guid PhaseDemandSkillId { get; set; }

    public Guid PhaseDemandId { get; set; }

    public Guid SkillId { get; set; }

    public string? PriorityLevel { get; set; }

    public string? Notes { get; set; }

    public virtual ImplementationTemplatePhaseDemand PhaseDemand { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}

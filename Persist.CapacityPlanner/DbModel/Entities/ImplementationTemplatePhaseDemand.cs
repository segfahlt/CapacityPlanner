using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class ImplementationTemplatePhaseDemand
{
    public Guid PhaseDemandId { get; set; }

    public Guid PhaseId { get; set; }

    public Guid RoleId { get; set; }

    public decimal? DefaultFte { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<ImplementationTemplatePhaseDemandSkill> ImplementationTemplatePhaseDemandSkill { get; set; } = new List<ImplementationTemplatePhaseDemandSkill>();

    public virtual ImplementationTemplatePhase Phase { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

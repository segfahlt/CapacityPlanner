using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Skill
{
    public Guid SkillId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DemandSkill> DemandSkill { get; set; } = new List<DemandSkill>();

    public virtual ICollection<ImplementationTemplatePhaseDemandSkill> ImplementationTemplatePhaseDemandSkill { get; set; } = new List<ImplementationTemplatePhaseDemandSkill>();

    public virtual ICollection<PersonSkill> PersonSkill { get; set; } = new List<PersonSkill>();
}

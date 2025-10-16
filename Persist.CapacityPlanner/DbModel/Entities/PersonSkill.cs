using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class PersonSkill
{
    public Guid PersonSkillId { get; set; }

    public Guid PersonId { get; set; }

    public Guid SkillId { get; set; }

    public string? ProficiencyLevel { get; set; }

    public string? Notes { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}

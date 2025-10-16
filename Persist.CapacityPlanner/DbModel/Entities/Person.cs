using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Person
{
    public Guid PersonId { get; set; }

    public string Name { get; set; } = null!;

    public string? EmploymentType { get; set; }

    public bool? IsActive { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<Actual> Actual { get; set; } = new List<Actual>();

    public virtual ICollection<Allocation> Allocation { get; set; } = new List<Allocation>();

    public virtual ICollection<PersonSkill> PersonSkill { get; set; } = new List<PersonSkill>();

    public virtual ICollection<Scenario> Scenario { get; set; } = new List<Scenario>();
}

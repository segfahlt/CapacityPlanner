using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Platform
{
    public Guid PlatformId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Implementation> Implementation { get; set; } = new List<Implementation>();

    public virtual ICollection<Module> Module { get; set; } = new List<Module>();
}

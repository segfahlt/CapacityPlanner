using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Module
{
    public Guid ModuleId { get; set; }

    public Guid PlatformId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Implementation> Implementation { get; set; } = new List<Implementation>();

    public virtual Platform Platform { get; set; } = null!;
}

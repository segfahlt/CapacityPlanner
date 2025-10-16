using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class ImplementationTemplate
{
    public Guid TemplateId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ImplementationTemplatePhase> ImplementationTemplatePhase { get; set; } = new List<ImplementationTemplatePhase>();
}

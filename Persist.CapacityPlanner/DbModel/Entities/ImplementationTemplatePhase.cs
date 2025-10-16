using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class ImplementationTemplatePhase
{
    public Guid PhaseId { get; set; }

    public Guid TemplateId { get; set; }

    public string PhaseName { get; set; } = null!;

    public int Sequence { get; set; }

    public int? DefaultDurationWeeks { get; set; }

    public int? DefaultStartOffsetWeeks { get; set; }

    public string? DefaultStatus { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<ImplementationTemplatePhaseDemand> ImplementationTemplatePhaseDemand { get; set; } = new List<ImplementationTemplatePhaseDemand>();

    public virtual ImplementationTemplate Template { get; set; } = null!;
}

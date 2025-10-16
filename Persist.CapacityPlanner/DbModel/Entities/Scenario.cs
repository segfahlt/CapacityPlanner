using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Scenario
{
    public Guid ScenarioId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Person? CreatedByNavigation { get; set; }

    public virtual ICollection<ForecastResult> ForecastResult { get; set; } = new List<ForecastResult>();
}

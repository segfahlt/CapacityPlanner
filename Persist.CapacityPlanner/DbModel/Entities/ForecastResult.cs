using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class ForecastResult
{
    public Guid ForecastId { get; set; }

    public Guid ScenarioId { get; set; }

    public Guid RoleId { get; set; }

    public DateOnly PeriodStart { get; set; }

    public DateOnly PeriodEnd { get; set; }

    public decimal RequiredFte { get; set; }

    public decimal AvailableFte { get; set; }

    public decimal? VarianceFte { get; set; }

    public decimal? UtilizationPct { get; set; }

    public string? Status { get; set; }

    public DateTime LastComputed { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Scenario Scenario { get; set; } = null!;
}

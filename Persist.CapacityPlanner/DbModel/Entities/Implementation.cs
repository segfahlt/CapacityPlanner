using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class Implementation
{
    public Guid ImplementationId { get; set; }

    public Guid PlatformId { get; set; }

    public Guid? ModuleId { get; set; }

    public string ClientName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? GoLiveDate { get; set; }

    public string? Status { get; set; }

    public virtual Module? Module { get; set; }

    public virtual Platform Platform { get; set; } = null!;

    public virtual ICollection<Workstream> Workstream { get; set; } = new List<Workstream>();
}

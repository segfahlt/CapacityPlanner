using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class LookupStatus
{
    public string Status { get; set; } = null!;

    public string? Description { get; set; }
}

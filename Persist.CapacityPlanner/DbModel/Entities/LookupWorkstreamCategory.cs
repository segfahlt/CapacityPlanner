using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class LookupWorkstreamCategory
{
    public string WorkstreamCategory { get; set; } = null!;

    public string? Description { get; set; }
}

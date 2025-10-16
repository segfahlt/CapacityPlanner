using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class LookupAllocationType
{
    public string AllocationType { get; set; } = null!;

    public string? Description { get; set; }
}

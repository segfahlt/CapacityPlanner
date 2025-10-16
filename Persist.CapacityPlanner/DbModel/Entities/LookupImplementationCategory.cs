using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class LookupImplementationCategory
{
    public string ImplementationCategory { get; set; } = null!;

    public string? Description { get; set; }
}

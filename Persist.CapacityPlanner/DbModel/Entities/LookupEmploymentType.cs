using System;
using System.Collections.Generic;

namespace Persist.CapacityPlanner.DbModel.Entities;

public partial class LookupEmploymentType
{
    public string EmploymentType { get; set; } = null!;

    public string? Description { get; set; }
}

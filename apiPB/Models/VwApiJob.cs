using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwApiJob
{
    public string Job { get; set; } = null!;

    public string? Description { get; set; }
}

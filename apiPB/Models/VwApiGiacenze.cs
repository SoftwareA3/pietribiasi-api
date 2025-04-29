using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwApiGiacenze
{
    public string Item { get; set; } = null!;

    public string? Description { get; set; }

    public string? BarCode { get; set; }

    public short FiscalYear { get; set; }

    public string Storage { get; set; } = null!;

    public double? BookInv { get; set; }
}

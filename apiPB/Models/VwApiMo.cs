using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwApiMo
{
    public string Job { get; set; } = null!;

    public short RtgStep { get; set; }

    public string Alternate { get; set; } = null!;

    public short AltRtgStep { get; set; }

    public string? Bom { get; set; }

    public string? Variant { get; set; }

    public string? ItemDesc { get; set; }

    public int Moid { get; set; }

    public string? Mono { get; set; }

    public string? Uom { get; set; }

    public double? ProductionQty { get; set; }

    public double? ProducedQty { get; set; }
}

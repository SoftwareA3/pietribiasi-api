using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwApiMostep
{
    public string Job { get; set; } = null!;

    public short RtgStep { get; set; }

    public string Alternate { get; set; } = null!;

    public short AltRtgStep { get; set; }

    public string? Wc { get; set; }

    public string? Operation { get; set; }

    public string Storage { get; set; } = null!;
}

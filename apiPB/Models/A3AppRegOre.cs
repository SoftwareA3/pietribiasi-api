using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class A3AppRegOre
{
    public int RegOreId { get; set; }

    public int WorkerId { get; set; }

    public DateTime? SavedDate { get; set; }

    public string? Job { get; set; }

    public short? RtgStep { get; set; }

    public string? Alternate { get; set; }

    public short? AltRtgStep { get; set; }

    public string Operation { get; set; } = null!;

    public string OperDesc { get; set; } = null!;

    public string Bom { get; set; } = null!;

    public string Variant { get; set; } = null!;

    public string? ItemDesc { get; set; }

    public int? Moid { get; set; }

    public string? Mono { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Uom { get; set; }

    public double ProductionQty { get; set; }

    public double ProducedQty { get; set; }

    public double ResQty { get; set; }

    public string? Storage { get; set; }

    public string? Wc { get; set; }

    public long? WorkingTime { get; set; }

    public bool Imported { get; set; }

    public string? UserImp { get; set; }

    public DateTime? DataImp { get; set; }

    public bool? Closed { get; set; }
}

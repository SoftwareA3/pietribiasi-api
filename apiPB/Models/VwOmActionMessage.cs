using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwOmActionMessage
{
    public int ActionId { get; set; }

    public int? Moid { get; set; }

    public short? RtgStep { get; set; }

    public string? Alternate { get; set; }

    public short? AltRtgStep { get; set; }

    public string? Mono { get; set; }

    public string? Bom { get; set; }

    public string? Variant { get; set; }

    public string? Wc { get; set; }

    public string? Operation { get; set; }

    public string? Job { get; set; }

    public int? WorkerId { get; set; }

    public int? ActionType { get; set; }

    public int? ActionStatus { get; set; }

    public string? ActionMessage { get; set; }

    public string? Closed { get; set; }

    public int? WorkerProcessingTime { get; set; }

    public int? WorkerSetupTime { get; set; }

    public int? ActualProcessingTime { get; set; }

    public int? ActualSetupTime { get; set; }

    public string? Storage { get; set; }

    public int? SpecificatorType { get; set; }

    public string? Specificator { get; set; }

    public string? ReturnMaterialQtyLower { get; set; }

    public string? PickMaterialQtyGreater { get; set; }

    public string? ProductionLotNumber { get; set; }

    public double? ProductionQty { get; set; }

    public DateTime Tbcreated { get; set; }

    public int TbcreatedId { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? ConfirmChildMos { get; set; }

    public int? Mostatus { get; set; }

    public int? MessageId { get; set; }

    public int? MessageType { get; set; }

    public DateTime? MessageDate { get; set; }

    public string? MessageText { get; set; }
}

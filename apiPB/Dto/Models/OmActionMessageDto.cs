using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class OmActionMessageDto
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

        public string? ActionType { get; set; }

        public string? ActionStatus { get; set; }

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

        public DateTime? DeliveryDate { get; set; }

        public string? ConfirmChildMos { get; set; }

        public string? Mostatus { get; set; }

        public int? MessageId { get; set; }

        public string? MessageType { get; set; }

        public DateTime? MessageDate { get; set; }

        public string? MessageText { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ActionMessageDetailsDto
    {
        public int ActionId { get; set; }
        public string? Job { get; set; }

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
    }
}
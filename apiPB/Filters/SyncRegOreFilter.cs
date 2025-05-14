using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class SyncRegOreFilter
    {
        public bool? Closed { get; set; }
        public int? WorkerId { get; set; }
        public int? MoId { get; set; }
        public short? RtgStep { get; set; }
        public string? Alternate { get; set; }
        public short? AltRtgStep { get; set; }
        public int ProducedQty { get; set; } = 0;
        public int SecondRateQty { get; set; } = 0;
        public int ScrapQty { get; set; } = 0;
        public TimeSpan? ActualProcessingTime { get; set; } // ORE REGISTRATE DA APP
        public TimeSpan? ActualSetupTime { get; set; } = new TimeSpan(0, 0, 0, 0); // "00:00:00:00"
        public TimeSpan? WorkerProcessingTime { get; set; } // ORE REGISTRATE DA APP
        public TimeSpan? WorkerSetupTime { get; set; } = new TimeSpan(0, 0, 0, 0); // "00:00:00:00"
        public string? SecondRate { get; set; } = string.Empty; // MANTIENI UGUALE
        public string? SecondRateStorage { get; set; } = "SEDE";
        public string? SecondRateVariant { get; set; } // PRENDI DA VARIANT
        public string? SecondRateNonConformityReason { get; set; } = string.Empty; // MANTIENI UGUALE
        public string? Scrap { get; set; } = string.Empty;
        public string? ScrapStorage { get; set; }
        public string? ScrapVariant { get; set; } // PRENDI DA VARIANT
        public string? ScrapNonConformityReason { get; set; } = string.Empty;
        public string? ProductionLotNumber { get; set; } = string.Empty;
        public string? Variant { get; set; } // PRENDI DA VARIANT 
        public string? Bom { get; set;} // INSERISCI BOM
        public string? Storage { get; set;} // INSERISCI SEDE
        public string? Wc { get; set;} // INSERISCI WC
        public string ExternalReferences { get;} = "";
        public bool PickMaterialQtyGreater{get;} = false;
        public bool ReturnMaterialQtyLower{get;} = false;
    }
}
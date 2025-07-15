using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per l'invio di infomazioni a Mago4 nel formato di sincronizzazione
    /// Contiene informazioni dettagliate sul materiale prelevato.
    /// </summary>
    public class SyncPrelMatDetailsRequestdto
    {
        public int? Position { get; set; }
        public double? PickedQty { get; set; } = 0;
        public bool? Closed { get; set; } = false;
        public int? SpecificatorType { get; set; }
        public string? Specificator { get; set; } = string.Empty;
        public string? Storage { get; set; } = string.Empty;
        public string? Lot { get; set; } = string.Empty;
        public double? NeededQty { get; set; } = 0;
        public string? Component { get; set; } = string.Empty;
        public string? ExternalProgram { get; set; } = "Pietribiasi App";
    }
}
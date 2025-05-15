using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class SyncPrelMatDetailsRequestdto
    {
        public int? Position { get; set; }
        public double? PickedQty { get; set; }
        public bool? Closed { get; set; } = false;
        public int? SpecificatorType { get; set; }
        public string? Specificator { get; set; } = string.Empty;
        public string? Storage { get; set; } = string.Empty;
        public string? Lot { get; set; } = string.Empty;
    }
}
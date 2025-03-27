using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto
{
    public class VwApiMocomponentDto
    {
        public string Job { get; set; } = null!;

        public int Moid { get; set; }

        public string? Mono { get; set; }

        public short Line { get; set; }

        public short? Position { get; set; }

        public short? ReferredPosition { get; set; }

        public string? Component { get; set; }

        public string? ComponentDesc { get; set; }

        public string? UoM { get; set; }

        public double? NeededQty { get; set; }

        public double? AssignedQuantity { get; set; }

        public double? PickedQuantity { get; set; }

        public string? Storage { get; set; }

        public string? Lot { get; set; }

        public int? SpecificatorType { get; set; }

        public string? Specificator { get; set; }

        public string? Closed { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class GiacenzeDto
    {
        public string Item { get; set; } = null!;

        public string? Description { get; set; }

        public string? BarCode { get; set; }

        public short FiscalYear { get; set; }

        public string Storage { get; set; } = null!;

        public double? BookInv { get; set; }
    }
}
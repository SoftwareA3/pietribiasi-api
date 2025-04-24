using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class PrelMatRequestDto : A3AppBaseRequestDto
    {
        public string? UoM { get; set; }
        public short? Position { get; set; }

        public string? Component { get; set; }

        public string BarCode { get; set; } = null!;

        public int? PrelQty { get; set; }
    }
}
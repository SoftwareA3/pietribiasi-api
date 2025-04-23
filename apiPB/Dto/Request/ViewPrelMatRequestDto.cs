using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewPrelMatRequestDto
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public string? Job { get; set; }
        public string? Operation { get; set; }
        public string? Mono { get; set; }
        public string? BarCode { get; set; }
    }
}
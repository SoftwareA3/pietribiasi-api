using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewPrelMatRequestDto : ViewOreRequestDto
    {
        public string? Component { get; set; }
        public string? BarCode { get; set; }
    }
}
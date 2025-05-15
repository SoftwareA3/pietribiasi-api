using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewInventarioRequestDto : A3AppViewRequestBaseDto
    {
        public string? Item { get; set; }
        public string? BarCode { get; set; }
    }
}
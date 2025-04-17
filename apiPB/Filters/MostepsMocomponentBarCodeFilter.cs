using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class MostepsMocomponentBarCodeFilter
    {
        public string Job { get; set; } = null!;

        public string? Mono { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Operation { get; set; }
        
        public string? BarCode { get; set; }
    }
}
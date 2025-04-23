using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class MostepOperationFilter
    {
        public string Job { get; set; } = null!;

        public required string Mono { get; set; }

        public required DateTime CreationDate { get; set; }

        public required string Operation { get; set; }
    }
}
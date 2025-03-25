using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto
{
    public class VwApiJobDto
    {
        public string Job { get; set; } = null!;

        public string? Description { get; set; }
    }
}
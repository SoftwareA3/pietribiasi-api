using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class JobDto
    {
        public string Job { get; set; } = null!;

        public string? Description { get; set; }
    }
}
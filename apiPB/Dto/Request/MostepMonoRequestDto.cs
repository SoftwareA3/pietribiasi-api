using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class MostepMonoRequestDto
    {
        public string Job { get; set; } = null!;
        
        public string? Mono { get; set; }

        public DateTime? CreationDate { get; set; }
    }
}
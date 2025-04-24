using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class MonoRequestDto : JobRequestDto
    {
        public required string Mono { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
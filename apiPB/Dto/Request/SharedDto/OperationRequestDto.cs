using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class OperationRequestDto : MonoRequestDto
    {
        public required string Operation { get; set; }
    }
}
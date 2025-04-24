using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class BarCodeRequestDto : OperationRequestDto
    {   
        public required string BarCode { get; set; }
    }
}
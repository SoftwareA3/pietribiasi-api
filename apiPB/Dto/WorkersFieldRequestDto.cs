using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto
{
    public class WorkersFieldRequestDto
    {
        public int WorkerId { get; set; }
        public short Line { get; set; }
        public string? FieldName { get; set; }
        public string? FieldValue { get; set; }
    }
}
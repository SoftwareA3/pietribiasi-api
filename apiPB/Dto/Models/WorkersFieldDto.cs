using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class WorkersFieldDto
    {
        public int WorkerId { get; set; }
        public short Line { get; set; }
        public string? FieldName { get; set; }
        public string? FieldValue { get; set; }
        public string? Notes { get; set; }
        public string? HideOnLayout { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewInventarioPutRequestDto
    {
        public int InvId { get; set; }
        public double BookInv { get; set; }
    }
}
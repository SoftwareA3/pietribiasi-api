using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewOrePutRequestDto
    {
        public int RegOreId { get; set; }
        public long WorkingTime { get; set; }
    }
}
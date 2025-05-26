using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class SyncronizedDataDto
    {
        public List<SyncPrelMatRequestDto> PrelMatRequest { get; set; } = new List<SyncPrelMatRequestDto>();
        public List<SyncRegOreRequestDto> RegOreRequest { get; set; } = new List<SyncRegOreRequestDto>();
        public List<SyncInventarioRequestDto> InventarioRequest { get; set; } = new List<SyncInventarioRequestDto>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Dto.Request
{
    public class SyncInventarioFilteredDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public List<InventarioDto> InventarioList { get; set; } = new List<InventarioDto>();
    }
}
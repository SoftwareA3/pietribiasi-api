using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Attacca il WorkerId alla lista di PrelMatDto filtrati in modo da sincronizzarne solo di determinati.
    /// </summary>
    public class SyncPrelMatFilteredDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public List<PrelMatDto> PrelMatList { get; set; } = new List<PrelMatDto>();
    }
}
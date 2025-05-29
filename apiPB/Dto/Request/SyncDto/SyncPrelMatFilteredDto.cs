using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace apiPB.Dto.Request
{
    public class SyncPrelMatFilteredDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public List<PrelMatDto> PrelMatDto { get; set; } = new List<PrelMatDto>();
    }
}
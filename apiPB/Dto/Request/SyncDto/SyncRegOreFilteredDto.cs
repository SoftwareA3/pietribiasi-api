using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Dto.Request
{
    public class SyncRegOreFilteredDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public List<RegOreDto> RegOreList { get; set; } = new List<RegOreDto>();
    }
}
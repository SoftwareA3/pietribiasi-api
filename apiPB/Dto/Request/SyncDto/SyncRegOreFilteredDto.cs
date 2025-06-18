using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Attacca il WorkerId alla lista di RegOreDto filtrati in modo da sincronizzarne solo di determinati.
    /// </summary>
    public class SyncRegOreFilteredDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public List<RegOreDto> RegOreList { get; set; } = new List<RegOreDto>();
    }
}
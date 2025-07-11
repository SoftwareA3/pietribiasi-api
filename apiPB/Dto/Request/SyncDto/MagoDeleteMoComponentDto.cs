using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class MagoDeleteMoComponentDto
    {
        public WorkerIdSyncRequestDto WorkerIdSyncRequestDto { get; set; } = new WorkerIdSyncRequestDto();
        public IEnumerable<DeleteMoComponentRequestDto> Request { get; set; } = new List<DeleteMoComponentRequestDto>();
    }
}
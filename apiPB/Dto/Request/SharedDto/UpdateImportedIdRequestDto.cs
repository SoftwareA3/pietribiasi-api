using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class UpdateImportedIdRequestDto : WorkerIdSyncRequestDto
    {
        public int Id { get; set; }
    }
}
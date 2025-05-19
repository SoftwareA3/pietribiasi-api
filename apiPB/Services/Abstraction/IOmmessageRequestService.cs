using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IOmmessageRequestService
    {
        List<OmmessageInfoRequestDto> GetOmmessageGroupedByMoid(MoIdRequestDto moIdRequestDtos);
    }
}
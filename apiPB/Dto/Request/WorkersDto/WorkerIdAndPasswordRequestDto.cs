using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class WorkerIdAndPasswordRequestDto : PasswordWorkersRequestDto
    {
        public int WorkerId { get; set; }
    }
}
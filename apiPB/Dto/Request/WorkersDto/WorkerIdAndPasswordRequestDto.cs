using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO contenente ID e password di un lavoratore.
    /// </summary>
    public class WorkerIdAndPasswordRequestDto : PasswordWorkersRequestDto
    {
        public int WorkerId { get; set; }
    }
}
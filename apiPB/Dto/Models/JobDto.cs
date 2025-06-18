using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    /// <summary>
    /// DTO per le operazioni di lavoro salvate nella vista vw_api_job.
    /// Questa tabella è necessaria per il recupero delle informazioni relative alle commesse
    /// sia per la registrazione delle ore che per il prelievo di materiali.
    /// </summary>
    public class JobDto
    {
        public string Job { get; set; } = null!;

        public string? Description { get; set; }
    }
}
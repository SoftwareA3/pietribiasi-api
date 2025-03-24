using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto
{
    public class WorkerDto
    {
        public int WorkerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TipoUtente { get; set; } = string.Empty;
        public string StorageVersamenti { get; set; } = string.Empty;
        public string Storage { get; set; } = string.Empty;
        public string LastLogin { get; set; } = string.Empty;
    }
}
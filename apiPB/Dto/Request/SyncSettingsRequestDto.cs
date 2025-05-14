using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;

namespace apiPB.Dto.Request
{
    public class SyncSettingsRequestDto : SettingsDto
    {
        public int? WorkerId { get; set; }
    }
}
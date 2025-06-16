using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la ricerca dell'identificativo di un PrelMat da eliminare.
    /// </summary>
    public class ViewPrelMatDeleteRequestDto
    {
        public int PrelMatId { get; set; }
    }
}
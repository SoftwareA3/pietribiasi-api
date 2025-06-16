using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la ricerca dell'identificativo di un RegOre da eliminare.
    /// </summary>
    public class ViewOreDeleteRequestDto
    {
        public int RegOreId { get; set; }
    }
}
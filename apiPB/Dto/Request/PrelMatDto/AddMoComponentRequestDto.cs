using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class AddMoComponentRequestDto
    {
        public int? WorkerId { get; set; } 
        public int? MoId { get; set; } 
        public int? NeededQty { get; set; } = 0; // quantità necessaria
        public string Component { get; set; } = string.Empty; // codice del componente da aggiungere
    }
}
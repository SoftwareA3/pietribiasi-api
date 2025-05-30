using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero del record di inventario dalla tabella A3_app_inventario e i parametri per la modifica.
    /// </summary>
    public class ViewInventarioPutRequestDto
    {
        public int InvId { get; set; }
        public double BookInv { get; set; }
    }
}
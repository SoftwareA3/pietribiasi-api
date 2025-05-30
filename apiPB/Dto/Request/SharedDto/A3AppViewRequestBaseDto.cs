using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per l'invio di richieste di filtro.
    /// I parametri presenti sono parametri comuni a tutte le richieste di questo tipo 
    /// Le classi dei DTO specifici ereditano questa classe per ereditare i parametri comuni.
    /// </summary>
    public class A3AppViewRequestBaseDto
    {
        public int? WorkerId { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public DateTime? DataImp { get; set; }
        public bool? Imported { get; set; } = false; // Default false
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero delle informazioni dalla tabella A3_app_inventario.
    /// I parametri passati sono parametri opzionali di ricerca per filtrare i dati.
    /// La classe deriva da A3AppViewRequestBaseDto per ereditare gli attributi comuni
    /// </summary>

    public class ViewInventarioRequestDto : A3AppViewRequestBaseDto
    {
        public string? Item { get; set; }
        public string? BarCode { get; set; }
    }
}
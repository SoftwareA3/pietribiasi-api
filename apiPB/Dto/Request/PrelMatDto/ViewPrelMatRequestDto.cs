using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero delle informazioni dalla tabella A3_app_prel_mat.
    /// I parametri passati sono parametri opzionali di ricerca per filtrare i dati.
    /// La classe deriva da ViewOreRequestDto per ereditare gli attributi comuni
    /// </summary>
    public class ViewPrelMatRequestDto : ViewOreRequestDto
    {
        public string? Component { get; set; }
        public string? BarCode { get; set; }
    }
}
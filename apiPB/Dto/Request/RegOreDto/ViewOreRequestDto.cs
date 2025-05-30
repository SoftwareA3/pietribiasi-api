using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero delle informazioni dalla tabella A3_app_reg_ore.
    /// I parametri passati sono parametri opzionali di ricerca per filtrare i dati.
    /// La classe deriva da A3AppViewRequestBaseDto per ereditare gli attributi comuni
    /// </summary>
    public class ViewOreRequestDto : A3AppViewRequestBaseDto
    {
        public string? Job { get; set; }
        public string? Operation { get; set; }
        public string? Mono { get; set; }
    }
}
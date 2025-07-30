using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per l'invio di informazioni alla tabella A3_app_reg_ore per la creazione di un nuovo record.
    /// La classe deriva da A3AppBaseRequestDto per ereditare gli attributi comuni
    /// </summary>
    public class RegOreRequestDto : A3AppBaseRequestDto
    {
        public string? Uom { get; set; }
        public long? WorkingTime { get; set; }
        public bool? Closed { get; set; }
    }
}
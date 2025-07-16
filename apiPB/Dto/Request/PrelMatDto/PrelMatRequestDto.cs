using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per l'invio di informazioni alla tabella A3_app_prel_mat per la creazione di un nuovo record.
    /// La classe deriva da A3AppBaseRequestDto per ereditare gli attributi comuni
    /// </summary>
    public class PrelMatRequestDto : A3AppBaseRequestDto
    {
        public string? UoM { get; set; }
        public short? Position { get; set; }
        public string? Component { get; set; }
        public string BarCode { get; set; } = null!;
        public double? PrelQty { get; set; }
        public bool? Deleted { get; set; }
    }
}
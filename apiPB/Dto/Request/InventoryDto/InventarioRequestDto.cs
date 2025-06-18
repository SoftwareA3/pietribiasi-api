using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero delle informazioni di inventario dalla tabella A3_app_inventario.
    /// </summary>
    public class InventarioRequestDto
    {
        public int InvId { get; set; }

        public int WorkerId { get; set; }

        public DateTime SavedDate { get; set; }

        public string Item { get; set; } = null!;

        public string? Description { get; set; }

        public string? BarCode { get; set; }

        public short FiscalYear { get; set; }

        public string Storage { get; set; } = null!;
        public double? BookInv { get; set; }
        public double? PrevBookInv { get; set; }
        public string? UoM { get; set; }
        public double? BookInvDiff { get; set; }
        public bool? InvRsn { get; set; }
        public bool Imported { get; set; }

        public string? UserImp { get; set; }

        public DateTime? DataImp { get; set; }
    }
}
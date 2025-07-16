using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    /// <summary>
    /// DTO per il recupero delle informazioni relative alle operazioni di prelievo dei materiali salvate.
    /// Recupera le informazioni dalla tabella A3_app_prel_mat.
    /// </summary>
    public class PrelMatDto
    {
        public int PrelMatId { get; set; }

        public int WorkerId { get; set; }

        public DateTime? SavedDate { get; set; }

        public string? Job { get; set; }

        public short? RtgStep { get; set; }

        public string? Alternate { get; set; }

        public short? AltRtgStep { get; set; }

        public string Operation { get; set; } = null!;

        public string OperDesc { get; set; } = null!;

        public short? Position { get; set; }

        public string? Component { get; set; }

        public string Bom { get; set; } = null!;

        public string Variant { get; set; } = null!;

        public string? ItemDesc { get; set; }

        public int? Moid { get; set; }

        public string? Mono { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? UoM { get; set; }

        public double ProductionQty { get; set; }

        public double ProducedQty { get; set; }

        public double ResQty { get; set; }

        public string? Storage { get; set; }

        public string BarCode { get; set; } = null!;

        public string? Wc { get; set; }

        public double? PrelQty { get; set; }

        public bool Imported { get; set; }

        public string? UserImp { get; set; }

        public DateTime? DataImp { get; set; }
        public bool? Deleted { get; set; }
    }
}
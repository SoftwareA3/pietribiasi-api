using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    /// <summary>
    /// DTO per il recupero e la modifica delle impostazioni dell'applicazione.
    /// </summary>
    public class SettingsDto
    {
        public string? MagoUrl { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Company { get; set; }

        public int? SpecificatorType { get; set; }

        public bool? TerminaLavorazioniUtente { get; set; }

        public string? RectificationReasonPositive { get; set; }

        public string? RectificationReasonNegative { get; set; }

        public string? Storage { get; set; }
        public bool? SyncGlobalActive { get; set; }
        public string? ExternalReferences { get; set; }
        public string? ExternalProgram { get; set; }
    }
}
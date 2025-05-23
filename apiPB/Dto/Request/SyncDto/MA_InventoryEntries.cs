using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace apiPB.Dto.Request
{
    public class MA_InventoryEntries
    {
        [JsonPropertyName("InvRsn")]
        public string? InvRsn { get; set; }
        [JsonPropertyName("PostingDate")]
        public string? PostingDate { get; set; } // Data di sincronizzazione
        [JsonPropertyName("PreprintedDocNo")]
        public string? PreprintedDocNo { get; set; }
        [JsonPropertyName("DocumentDate")]
        public string? DocumentDate { get; set; } // Data di salvataggio
        [JsonPropertyName("StoragePhase1")]
        public string? StoragePhase1 { get; set; } // Storage 
    }
}
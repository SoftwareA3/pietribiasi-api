using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace apiPB.Dto.Request
{
    public class MA_InventoryEntriesDetail
    {
        [JsonPropertyName("Item")]
        public string? Item { get; set; }
        [JsonPropertyName("Qty")]
        public double? Qty { get; set; } // Differenza
        [JsonPropertyName("UoM")]
        public string? UoM { get; set; } // Unità di misura
        [JsonPropertyName("UnitValue")]
        public int UnitValue { get; set; } = 0; // Lascia a 0
        [JsonPropertyName("DocumentType")]
        public int DocumentType { get; set; } = 3801188; // Lascia a 3801188
    }
}
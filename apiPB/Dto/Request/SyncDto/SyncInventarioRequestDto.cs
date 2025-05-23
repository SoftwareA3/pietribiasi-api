using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace apiPB.Dto.Request
{
    public class SyncInventarioRequestDto
    {
        [JsonPropertyName("MA_InventoryEntries")]
        public MA_InventoryEntries MA_InventoryEntries { get; set; } = null!;
        [JsonPropertyName("MA_InventoryEntriesDetail")]
        public List<MA_InventoryEntriesDetail> MA_InventoryEntriesDetail { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per l'invio di infomazioni a Mago4 nel formato di sincronizzazione
    /// Incapsula le informazioni da inviare per sincronizzare l'elemento dell'inventario in un unico DTO
    /// in modo da poter inviare simultanteamente le informazioni.
    /// </summary>
    public class SyncInventarioRequestDto
    {
        [JsonPropertyName("MA_InventoryEntries")]
        public MA_InventoryEntries MA_InventoryEntries { get; set; } = null!;
        [JsonPropertyName("MA_InventoryEntriesDetail")]
        public List<MA_InventoryEntriesDetail> MA_InventoryEntriesDetail { get; set; } = null!;
    }
}
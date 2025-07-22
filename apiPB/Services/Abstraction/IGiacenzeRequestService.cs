using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IGiacenzeRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella vw_api_giacenze
        /// </summary>
        /// <returns>
        /// IEnumerable di GiacenzeDto: restituisce una collezione generica di GiacenzeDto
        /// </returns>
        IEnumerable<GiacenzeDto> GetGiacenze();

        GiacenzeDto GetGiacenzeByItem(ComponentRequestDto request);
    }
}
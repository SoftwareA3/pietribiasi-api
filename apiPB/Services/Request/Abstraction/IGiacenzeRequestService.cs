using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
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
    }
}
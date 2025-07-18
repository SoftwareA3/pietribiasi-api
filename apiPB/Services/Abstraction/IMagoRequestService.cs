using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    /*
    * Unico servizio più complicato: la responsabilità è la stessa,
    * ma quando si tratta della sincronizzazione generale, deve occuparsi di chiamare (in SyncronizeAsync):
    * Repository, Mapper, Servizio e Repository per tutte e 3 le entità (RegOre, PrelMat, Inventario).
    * Inoltre, siccome le eccezioni non dipendono dai repository, ma dalla chiamata a Mago,
    * è necessario gestire le eccezioni in modo centralizzato per poi spostarle al controller.
    */
    public interface IMagoRequestService
    {
        /// <summary>
        /// Effettua la sincronizzazione dei dati salvati con i dati di Mago
        /// </summary>
        /// <param name="request">Id del lavoratore che effettua la sincronizzazione</param>
        /// <param name="responseDto">Risposta del login a Mago</param>
        /// <param name="settings">Impostazioni di sincronizzazione</param>
        /// <returns>Ritorna i dati che sono stati sincronizzati</returns>
        Task<SyncronizedDataDto> SyncronizeAsync(MagoLoginResponseDto responseDto, SettingsDto settings, WorkerIdSyncRequestDto requestId);

        /// <summary>
        /// Effettua la sincronizzazione delle ore registrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle ore registrate</param>
        /// <param name="token">Token di autenticazione</param>
        /// <returns>Ritorna le ore registrate sincronizzate</returns>
        Task<HttpResponseMessage> SyncRegOre(IEnumerable<SyncRegOreRequestDto> request, string token);

        /// <summary>
        /// Effettua la sincronizzazione delle informazioni dei prelievi effettuati
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle informazioni dei materiali prelevati</param>
        /// <param name="token">Token di autenticazione</param>
        /// <returns>Ritorna le informazioni dei materiali prelevati sincronizzati</returns>
        Task<HttpResponseMessage> SyncPrelMat(IEnumerable<SyncPrelMatRequestDto> request, string token);

        /// <summary>
        /// Effettua la sincronizzazione delle informazioni delle movimentazioni di inventario
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle informazioni delle movimentazioni di inventario</param>
        /// <param name="token">Token di autenticazione</param>
        /// <returns>Ritorna le informazioni delle movimentazioni di inventario sincronizzate</returns>
        Task<HttpResponseMessage> SyncInventario(IEnumerable<SyncInventarioRequestDto> request, string token);

        /// <summary>
        /// Effettua la sincronizzazione delle ore registrate filtrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle ore registrate filtrate</param>
        /// <param name="isFiltered">Indica se la richiesta viene fatta dalla pagina dell'operazione e con eventuali filtri</param>
        /// <param name="responseDto">Risposta del login a Mago</param>
        /// <param name="settings">Impostazioni di sincronizzazione</param>
        /// <returns>Ritorna le ore registrate sincronizzate filtrate</returns>
        Task<IEnumerable<SyncRegOreRequestDto>> SyncRegOreFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncRegOreFilteredDto? request, bool isFiltered = true);

        /// <summary>
        /// Effettua la sincronizzazione delle informazioni dei prelievi effettuati filtrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle informazioni dei materiali prelevati filtrate</param>
        /// <param name="isFiltered">Indica se la richiesta viene fatta dalla pagina dell'operazione e con eventuali filtri</param>
        /// <param name="responseDto">Risposta del login a Mago</param>
        /// <param name="settings">Impostazioni di sincronizzazione</param>
        /// <returns>Ritorna le informazioni dei materiali prelevati sincronizzati filtrati</returns>
        Task<IEnumerable<SyncPrelMatRequestDto>> SyncPrelMatFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncPrelMatFilteredDto? request, bool isFiltered = true);

        /// <summary>
        /// Effettua la sincronizzazione delle informazioni delle movimentazioni di inventario filtrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle informazioni delle movimentazioni di inventario filtrate</param>
        /// <param name="isFiltered">Indica se la richiesta viene fatta dalla pagina dell'operazione e con eventuali filtri</param>
        /// <param name="responseDto">Risposta del login a Mago</param>
        /// <param name="settings">Impostazioni di sincronizzazione</param>
        /// <returns>Ritorna le informazioni delle movimentazioni di inventario sincronizzate filtrate</returns>
        Task<IEnumerable<SyncInventarioRequestDto>> SyncInventarioFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncInventarioFilteredDto? request, bool isFiltered = true);

        /// <summary>
        /// Effettua la procedura di login per l'autenticazione a Mago
        /// </summary>
        /// <param name="request">Richiesta di login</param>
        /// <returns>Ritorna le informazioni restituite dal login, come il token</returns>
        Task<MagoLoginResponseDto?> LoginAsync(MagoLoginRequestDto request);

        /// <summary>
        /// Effettua la procedura di login per l'autenticazione a Mago con WorkerId
        /// </summary>
        /// <param name="request">Richiesta di login con WorkerId</param>
        /// <returns>Ritorna le informazioni restituite dal login, come il token</returns>
        /// <remarks>
        /// Questo metodo è utilizzato per chiamare LoginAsync utilizzando esclusivamente il WorkerId.
        /// </remarks>
        Task<(MagoLoginResponseDto? LoginResponse, SettingsDto? Settings)> LoginWithWorkerIdAsync(WorkerIdSyncRequestDto request);

        /// <summary>
        /// Effettua la procedura di logoff per disconnettersi da Mago
        /// </summary>
        /// <param name="requestToken">Richiesta di logoff</param>
        Task LogoffAsync(MagoTokenRequestDto requestToken);

        Task<DeleteMoComponentRequestDto> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, DeleteMoComponentRequestDto request);
    }
}
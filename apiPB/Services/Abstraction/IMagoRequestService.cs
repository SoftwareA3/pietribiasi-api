using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IMagoRequestService
    {
        /// <summary>
        /// Effettua la sincronizzazione dei dati salvati con i dati di Mago
        /// </summary>
        /// <param name="request">Id del lavoratore che effettua la sincronizzazione</param>
        /// <returns>Ritorna i dati che sono stati sincronizzati</returns>
        Task<SyncronizedDataDto> SyncronizeAsync(WorkerIdSyncRequestDto request);

        /// <summary>
        /// Modifica le impostazioni per la connessione a Mago e la sincronizzazione
        /// </summary>
        /// <param name="settings">Parametri per la connessione a Mago e parametri di default</param>
        /// <returns></returns>
        SettingsDto? EditSettings(SettingsDto settings);

        /// <summary>
        /// Ritorna le impostazioni per la connessione a Mago e la sincronizzazione
        /// </summary>
        /// <returns>Ritorna le impostazioni per la connessione a Mago e la sincronizzazione</returns>
        SettingsDto? GetSettings();

        /// <summary>
        /// Ritorna le impostazioni per la sincronizzazione globale
        /// </summary>
        /// <returns>Ritorna le impostazioni per la sincronizzazione globale</returns>
        SyncGobalActiveRequestDto? GetSyncGlobalActive();

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
        /// Effettua la procedura di login per l'autenticazione a Mago
        /// </summary>
        /// <param name="request">Richiesta di login</param>
        /// <returns>Ritorna le informazioni restituite dal login, come il token</returns>
        Task<MagoLoginResponseDto?> LoginAsync(MagoLoginRequestDto request);

        /// <summary>
        /// Effettua la procedura di logoff per disconnettersi da Mago
        /// </summary>
        /// <param name="requestToken">Richiesta di logoff</param>
        Task LogoffAsync(MagoTokenRequestDto requestToken);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface ISettingsRequestService
    {
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

        TerminaLavorazioniUtenteRequestDto? GetTerminaLavorazioniUtente();

        ControlloUoMRequestDto? GetControlloUoM();
    }
}
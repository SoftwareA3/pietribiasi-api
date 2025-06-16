using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.ApiClient.Abstraction
{
    /// <summary>
    /// Questa classe si occupa della creazione di un client per le API di Mago4.
    /// Viene utilizzata per la costruzione delle richieste HTTP verso gli endpoint di Mago4.
    /// Viene impostato l'header e le particolarità del corpo della richiesta.
    /// </summary>
    public interface IMagoApiClient
    {
        /// <summary>
        /// Invia una richiesta POST a un endpoint specificato con un corpo di tipo T e un token di autenticazione.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <param name="token"></param>
        /// <param name="isList">Specifica se trattare il parametro come lista o elemento. Nel caso sia false, 
        /// il metodo estrarrà il primo elemento dalla lista passata</param>
        /// <returns>Codice di risposta ritornato da Mago4</returns>
        Task<HttpResponseMessage> SendPostAsyncWithToken<T>(string endpoint, IEnumerable<T> body, string token, bool isList = true);

        /// <summary>
        /// Invia una richiesta POST a un endpoint specificato con un corpo di tipo object.
        /// </summary>
        /// <param name="endpoint">Porzione di URL contenente l'enpoint della richiesta</param>
        /// <param name="body">Corpo della richiesta da inviare</param>
        /// <returns>Codice di risposta ritornato da Mago4</returns>
        Task<HttpResponseMessage> SendPostAsync(string endpoint, object body);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Utils.Abstraction
{
    public interface ILogService
    {
        /// <summary>
        /// Metodo che aggiunge un messaggio al file di log, passando informazioni specifiche
        /// </summary>
        /// <param name="requestType">Tipo di richiesta e percorso di questa. Ad esempio: GET api/user</param>
        /// <param name="statusCode">Numero di stato della richiesta. Ad esempio: 200</param>
        /// <param name="statusMessage">Messaggio dello stato della richiesta. Ad esempio: Ok</param>
        /// <param name="isActive">Controlla se il log è attivo o meno</param>
        /// <remarks>Il log è attivo se isActive è true</remarks>
        void AppendMessageToLog(string requestType, int? statusCode, string statusMessage, bool isActive);

        void AppendWarningToLog(string warningMessage);

        void AppendErrorToLog(string errorMessage);

        /// <summary>
        /// Chiama il metodo AppendMessageToLog e aggiunge una lista di oggetti generici al file di log
        /// </summary>
        /// <typeparam name="T">Nome del tipo di classe o oggetto passato in una lista generica</typeparam>
        /// <param name="requestType">Tipo di richiesta e percorso di questa. Ad esempio: GET api/user</param>
        /// <param name="statusCode">Numero di stato della richiesta. Ad esempio: 200</param>
        /// <param name="statusMessage">Messaggio dello stato della richiesta. Ad esempio: Ok</param>
        /// <param name="list">Lista di tipo generico</param>
        /// <param name="isActive">Controlla se il log è attivo o meno</param>
        /// <remarks>Il log è attivo se isActive è true</remarks>
        void AppendMessageAndListToLog<T>(string requestType, int? statusCode, string statusMessage, List<T> list, bool isActive);

        /// <summary>
        /// Chiama il metodo AppendMessageToLog e aggiunge un oggetto generico al file di log
        /// </summary>
        /// <typeparam name="T">Nome del tipo di classe o oggetto passato</typeparam>
        /// <param name="requestType">Tipo di richiesta e percorso di questa. Ad esempio: GET api/user</param>
        /// <param name="statusCode">Numero di stato della richiesta. Ad esempio: 200</param>
        /// <param name="statusMessage">Messaggio dello stato della richiesta. Ad esempio: Ok</param>
        /// <param name="item">Oggetto generico</param>
        /// <param name="isActive">Controlla se il log è attivo o meno</param>
        /// <remarks>Il log è attivo se isActive è true</remarks>
        void AppendMessageAndItemToLog<T>(string requestType, int? statusCode, string statusMessage, T item, bool isActive);
    }
}
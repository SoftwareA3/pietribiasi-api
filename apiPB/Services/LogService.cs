using System.Net;
using System.Reflection;

namespace apiPB.Services
{
    // Classe che si occupa della creazione della directory e del file di log
    // Aggiunge dei metodi per l'inserimento di informazioni nel file di log
    public class LogService
    {
        private readonly string _logFolderPath = string.Empty;
        private readonly string _logFilePath = string.Empty;
        public LogService()
        {
            _logFolderPath = "Logs\\";
            _logFilePath = Path.Combine(_logFolderPath, "API.log");
        }

        // Metodo per la creazione della cartella di log
        private void CreateDirectory()
        {
            if (!Directory.Exists(_logFolderPath))
            {
                Directory.CreateDirectory(_logFolderPath);
            }
        }

        // Metodo per la creazione del file di log
        private void CreateLogFile()
        {
            CreateDirectory();
            if (!File.Exists(_logFilePath))
            {
                using (File.Create(_logFilePath))
                {
                };
            }
        }

        // Metodo che restituisce la stringa contenente l'indirizzo IP
        private string AppendIpAddress()
        {
            string ipAddress = string.Empty;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = address.ToString();
                    break;
                }
            }

            return ipAddress;
        } 

        /// <summary>
        /// Metodo che aggiunge un messaggio al file di log, passando informazioni specifiche
        /// </summary>
        /// <param name="requestType">Tipo di richiesta e percorso di questa. Ad esempio: GET api/user</param>
        /// <param name="statusCode">Numero di stato della richiesta. Ad esempio: 200</param>
        /// <param name="statusMessage">Messaggio dello stato della richiesta. Ad esempio: Ok</param>
        /// <param name="isActive">Controlla se il log è attivo o meno</param>
        /// <remarks>Il log è attivo se isActive è true</remarks>
        public void AppendMessageToLog(string requestType, int? statusCode, string statusMessage, bool isActive = false)
        {
            if(isActive == false)
            {
                return;
            }
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{requestType}] - StatusCode: {statusCode} - Message: {statusMessage}";
            
            writer.WriteLine(message);
        }

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
        public void AppendMessageAndListToLog<T>(string requestType, int? statusCode, string statusMessage, List<T> list, bool isActive = false)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage, isActive);
            // Se isActive è false, non eseguire AppendListToLog
            // Necessario perché in AppendMessageToLog viene controllato, ma semplicemente ritorna senza eseguire
            if(isActive == false)
            {
                return;
            }
            if (list != null)
            {
                AppendListToLog(list);
            }
        }

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
        public void AppendMessageAndItemToLog<T>(string requestType, int? statusCode, string statusMessage, T item, bool isActive = false)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage, isActive);
            if(isActive == false)
            {
                return;
            }
            if (item != null)
            {
                AppendItemToLog(item);
            }
        }

        // Metodo che aggiunge una lista di oggetti al file di log
        // Questo metodo riceve una lista generica come parametro
        private void AppendListToLog<T>(List<T> list)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);

            // Itera sugli elementi della lista
            foreach (var item in list)
            {  
                // Ottiene le proprietà dell'oggetto attraverso la riflessione
                PropertyInfo[] property = typeof(T).GetProperties();

                writer.Write("\t");

                // Itera sulle proprietà dell'oggetto
                foreach (var p in property)
                {
                    var value = p.GetValue(item); 
                    writer.Write($"{p.Name}: {value}");

                    if (p != property.Last())
                    {
                        writer.Write(" - ");
                    }
                }
                writer.WriteLine();
            }

            writer.WriteLine();
        }

        // Metodo che aggiunge un oggetto al file di log
        // Questo metodo riceve un oggetto generico come parametro
        private void AppendItemToLog<T>(T item)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);

            PropertyInfo[] property = typeof(T).GetProperties();

            writer.Write("\t");

            foreach (var p in property)
            {
                var value = p.GetValue(item); 
                writer.Write($"{p.Name}: {value}");

                if (p != property.Last())
                {
                    writer.Write(" - ");
                }
            }
            writer.WriteLine();
        }
    }
}
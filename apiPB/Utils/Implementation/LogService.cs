using System.Net;
using System.Reflection;
using apiPB.Utils.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using apiPB.Utils.Implementation;
using apiPB.Repository.Abstraction;

namespace apiPB.Utils.Implementation
{
    // Classe che si occupa della creazione della directory e del file di log
    // Aggiunge dei metodi per l'inserimento di informazioni nel file di log
    public class LogService : ILogService
    {
        private readonly string _logFolderPath = string.Empty;
        private readonly string _logFilePath = string.Empty;
        private readonly string _logErrorFilePath = string.Empty;

        private readonly ISettingsRepository _settingsRepository;
        public LogService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _logFolderPath = "..\\Logs\\";
            _logFilePath = Path.Combine(_logFolderPath, "API.log");
            _logErrorFilePath = Path.Combine(_logFolderPath, "APIErrors.log");
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
                }
                ;
            }
        }

        private void CreateErrorLogFile()
        {
            CreateDirectory();
            if (!File.Exists(_logErrorFilePath))
            {
                using (File.Create(_logErrorFilePath))
                {
                }
                ;
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

        public void AppendMessageToLog(string requestType, int? statusCode, string statusMessage)
        {
            if (IsLogEnabled() == false)
            {
                return;
            }
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{requestType}] - StatusCode: {statusCode} - Message: {statusMessage}";

            writer.WriteLine(message);
        }

        public void AppendErrorToLog(string errorMessage)
        {
            CreateErrorLogFile();

            using var fileStream = new FileStream(_logErrorFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"===== ERROR =====\n{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - ErrorMessage:\n{errorMessage}\n===== END ERROR =====\n";

            // Scrive il messaggio di errore nella console
            Console.WriteLine(message);

            // Scrive il messaggio di errore nel file di log
            writer.WriteLine(message);
        }

        public void AppendWarningToLog(string warningMessage)
        {
            CreateErrorLogFile();

            using var fileStream = new FileStream(_logErrorFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"===== WARNING =====\n{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - WarningMessage:\n{warningMessage}\n===== END WARNING =====\n";

            // Scrive il messaggio di attenzione nella console
            Console.WriteLine(message);

            // Scrive il messaggio di attenizione nel file di log
            writer.WriteLine(message);
        }


        public void AppendMessageAndListToLog<T>(string requestType, int? statusCode, string statusMessage, List<T> list)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage);
            // Se isActive è false, non eseguire AppendListToLog
            // Necessario perché in AppendMessageToLog viene controllato, ma semplicemente ritorna senza eseguire
            if (IsLogEnabled() == false)
            {
                return;
            }
            if (list != null)
            {
                AppendListToLog(list);
            }
        }

        public void AppendMessageAndItemToLog<T>(string requestType, int? statusCode, string statusMessage, T item)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage);
            if (IsLogEnabled() == false)
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
        
        public bool IsLogEnabled()
        {
            var abilitaLogResult = _settingsRepository.GetAbilitaLog();
            return abilitaLogResult != null && abilitaLogResult.AbilitaLog == true;
        }
    }
}
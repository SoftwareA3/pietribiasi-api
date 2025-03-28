using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using apiPB.Models;
using System.Collections.Generic;
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

        // Metodo che aggiunge un messaggio al file di log, passando informazioni specifiche
        public void AppendMessageToLog(string requestType, int? statusCode, string statusMessage)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{requestType}] - StatusCode: {statusCode} - Message: {statusMessage}";
            
            writer.WriteLine(message);
        }

        public void AppendMessageAndListToLog<T>(string requestType, int? statusCode, string statusMessage, List<T> list)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage);
            if (list != null)
            {
                AppendListToLog(list);
            }
        }

        public void AppendMessageAndItemToLog<T>(string requestType, int? statusCode, string statusMessage, T item)
        {
            AppendMessageToLog(requestType, statusCode, statusMessage);
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
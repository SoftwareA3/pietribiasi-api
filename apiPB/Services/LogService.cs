using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace apiPB.Services
{
    public class LogService
    {
        private readonly IConfiguration _configuration;
        private readonly string _logFolderPath = string.Empty;
        private readonly string _logFilePath = string.Empty;
        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _logFolderPath = _configuration.GetValue<string>("LogFolderPath") ?? string.Empty;
            _logFilePath = Path.Combine(_logFolderPath, "API.log");
        }

        private void CreateDirectory()
        {
            // Creazione della cartella di log
            if (!Directory.Exists(_logFolderPath))
            {
                Directory.CreateDirectory(_logFolderPath);
            }
        }

        private void CreateLogFile()
        {
            CreateDirectory();
            // Creazione del file di log
            if (!File.Exists(_logFilePath))
            {
                using (File.Create(_logFilePath))
                {
                };
            }
        }

        // Invece che message generico, info specifiche
        // Time: {DateTime.Now}; POST api/worker; StatusCode: {nf.StatusCode}; Message: Not Found;
        // DateTime.Now è scontato
        public void AppendMessageToLog(string requestType, int? statusCode, string statusMessage)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            string message = $"{AppendIpAddress()} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {requestType} - StatusCode: {statusCode} - Message: {statusMessage}";
            
            writer.WriteLine(message);
        }

        public void AppendWorkerListToLog(List<WorkerDto> workers)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            foreach (var worker in workers)
            {
                writer.WriteLine($"\tWorkerId: {worker.WorkerId} - Name: {worker.Name} - LastName: {worker.LastName} - Pin: {worker.Pin} - Password: {worker.Password} - TipoUtente: {worker.TipoUtente} - StorageVersamenti: {worker.StorageVersamenti} - Storage: {worker.Storage} - LastLogin: {worker.LastLogin}");
            }
        }
    

        public void AppendWorkersFieldListToLog(List<RmWorkersFieldDto> workersFields)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            foreach (var workerField in workersFields)
            {
                writer.WriteLine($"\tWorkerId: {workerField.WorkerId} - Line: {workerField.Line} - FieldName: {workerField.FieldName} - FieldValue: {workerField.FieldValue} - Notes: {workerField.Notes} - HideOnLayout: {workerField.HideOnLayout} - Tbcreated: {workerField.Tbcreated} - Tbmodified: {workerField.Tbmodified} - TbcreatedId: {workerField.TbcreatedId} - TbmodifiedId: {workerField.TbmodifiedId}");
            }
            writer.WriteLine("\n");
        }

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
    }
}
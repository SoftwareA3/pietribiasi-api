using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto;

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
            _logFilePath = Path.Combine(_logFolderPath, "log.txt");
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
                File.Create(_logFilePath);
            }
        }

        public void AppendMessageToLog(string message)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);
            writer.WriteLine(message);
        }

        public void AppendWorkerListToLog(List<WorkerDto> workers)
        {
            CreateLogFile();

            using var fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);
            foreach (var worker in workers)
            {
                writer.WriteLine($"\tWorkerId: {worker.WorkerId}; Name: {worker.Name}; LastName: {worker.LastName}; Pin: {worker.Pin}; Password: {worker.Password}; TipoUtente: {worker.TipoUtente}; StorageVersamenti: {worker.StorageVersamenti}; Storage: {worker.Storage}; LastLogin: {worker.LastLogin}");
            }
            writer.WriteLine("\n");
        }
    

        public void AppendWorkersFieldListToLog(List<RmWorkersFieldDto> workersFields)
        {
            CreateLogFile();
            var logFilePath = Path.Combine(_logFolderPath, "log.txt");

            using var fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fileStream);
            foreach (var workerField in workersFields)
            {
                writer.WriteLine($"\tWorkerId: {workerField.WorkerId}; Line: {workerField.Line}; FieldName: {workerField.FieldName}; FieldValue: {workerField.FieldValue}; Notes: {workerField.Notes}; HideOnLayout: {workerField.HideOnLayout}; Tbcreated: {workerField.Tbcreated}; Tbmodified: {workerField.Tbmodified}; TbcreatedId: {workerField.TbcreatedId}; TbmodifiedId: {workerField.TbmodifiedId}");
            }
            writer.WriteLine("\n");
        }
    }
}
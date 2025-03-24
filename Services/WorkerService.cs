using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using apiPB.Models;
using apiPB.Data;
using apiPB.Dto;

namespace apiPB.Services
{
    public class WorkerService
    {
        private readonly ApplicationDbContext _context;

        public WorkerService(ApplicationDbContext context)
        {
            _context = context;
        }
        // public List<VwWorker> GetAll()
        // {
        //     // Questa stringa viene usata come query per recuperare le informazioni dei lavoratori dal database
        //     var query = @"
        //         SELECT 
        //             w.WorkerID, 
        //             w.Name, 
        //             w.LastName, 
        //             w.Pin, 
        //             WFP.FieldValue AS Password, 
        //             WFT.FieldValue AS TipoUtente, 
        //             ISNULL(SSV.Storage, 'SEDE') AS StorageVersamenti, 
        //             ISNULL(SSP.Storage, 'SEDE') AS Storage,
        //             ISNULL(WFSPQ.FieldValue, '') LastLogin 
        //         FROM RM_Workers W
        //         INNER JOIN RM_WorkersFields WFP 
        //             ON W.WorkerID = WFP.WorkerID AND WFP.FieldName = 'Password Versamenti'
        //         INNER JOIN RM_WorkersFields WFT 
        //             ON W.WorkerID = WFT.WorkerID AND WFT.FieldName = 'Tipo Utente'
        //         LEFT OUTER JOIN RM_WorkersFields WFSV 
        //             ON W.WorkerID = WFSV.WorkerID AND WFSV.FieldName = 'Divisione'
        //         LEFT OUTER JOIN MA_Storages SSV 
        //             ON WFSV.FieldValue = SSV.Storage
        //         LEFT OUTER JOIN RM_WorkersFields WFSP 
        //             ON W.WorkerID = WFSP.WorkerID AND WFSP.FieldName = 'Deposito Prelievo'
        //         LEFT OUTER JOIN MA_Storages SSP 
        //             ON WFSP.FieldValue = SSP.Storage
        //         LEFT OUTER JOIN RM_WorkersFields WFSPQ 
        //             ON W.WorkerID = WFSPQ.WorkerID AND WFSPQ.FieldName = 'Last Login'";

        //     var results = new List<VwWorker>();

        //     using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        //     {
        //         connection.Open();
        //         using (var command = new SqlCommand(query, connection))
        //         {
        //             using (var reader = command.ExecuteReader())
        //             {
        //                 // Recupera tutte le entry del database e le inserisce in una lista di VW_Workers
        //                 while (reader.Read())
        //                 {
        //                     results.Add(new VwWorker
        //                     {
        //                         WorkerId = reader.GetInt32(0),
        //                         Name = reader.GetString(1),
        //                         LastName = reader.GetString(2),
        //                         Pin = reader.GetString(3),
        //                         Password = reader.GetString(4),
        //                         TipoUtente = reader.GetString(5),
        //                         StorageVersamenti = reader.GetString(6),
        //                         Storage = reader.GetString(7),
        //                         LastLogin = reader.GetString(8)
        //                     });
        //                 }
        //             }
        //         }
        //     }

        //     return results;
        // }

        public RmWorkersField? GetWorkerFieldsLastLineFromWorkerId(int workerId)
        {
            // Dalla tabella WorkersFields, viene preso il numero massimo di Line del workerId
            var query = @"
                SELECT TOP 1 * 
                FROM RM_WorkersFields 
                WHERE WorkerID = " + workerId + 
                " ORDER BY Line DESC";    
            
            // Viene aperta una connessione al database per eseguire la query e creare un oggetto RmWorkersField da ritornare
            using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            Console.WriteLine(_context.Database.GetDbConnection().ConnectionString);
            connection.Open();
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Il reader legge i campi che vengono assengati ad un nuovo
                // oggetto RmWorkersField che viene ritornato per essere usato dal controller
                return new RmWorkersField
                {
                    WorkerId = reader.GetInt32(0),
                    Line = reader.GetInt16(1),
                    FieldName = reader.GetString(2),
                    FieldValue = reader.GetString(3),
                    Notes = reader.GetString(4),
                    HideOnLayout = reader.GetString(5),
                    Tbcreated = reader.GetDateTime(6),
                    Tbmodified = reader.GetDateTime(7),
                    TbcreatedId = reader.GetInt32(8),
                    TbmodifiedId = reader.GetInt32(9)
                };
            }
            return null;
        }
    }
}
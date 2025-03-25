using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using apiPB.Dto;
using apiPB.Models;

namespace apiPB.Mappers
{
    // Classe che mappa il parametro per il tipo ritornato, in modo che il controller possa restituire un oggetto con specifiche informazioni
    public static class WorkerMapper
    {
        public static VwApiWorkerDto ToWorkerDto(this VwApiWorker workerModel)
        {
            return new VwApiWorkerDto
            {
                WorkerId = workerModel.WorkerId,
                Name = workerModel.Name ?? string.Empty,
                LastName = workerModel.LastName ?? string.Empty,
                Pin = workerModel.Pin ?? string.Empty,
                Password = workerModel.Password ?? string.Empty,
                TipoUtente = workerModel.TipoUtente ?? string.Empty,
                StorageVersamenti = workerModel.StorageVersamenti,
                Storage = workerModel.Storage,
                LastLogin = workerModel.LastLogin
            };
        }

        public static RmWorkersFieldDto ToWorkersFieldDto(this RmWorkersField workersField)
        {
            return new RmWorkersFieldDto
            {
                WorkerId = workersField.WorkerId,
                Line = workersField.Line,
                FieldName = workersField.FieldName,
                FieldValue = workersField.FieldValue,
                Notes = workersField.Notes,
                HideOnLayout = workersField.HideOnLayout,
                Tbcreated = workersField.Tbcreated,
                Tbmodified = workersField.Tbmodified,
                TbcreatedId = workersField.TbcreatedId,
                TbmodifiedId = workersField.TbmodifiedId
            };
        }

        public static RmWorkersField ToWorkersFieldFromCreateDto(this RmWorkersFieldDto workersFieldRequestDto)
        {
            return new RmWorkersField
            {
                WorkerId = workersFieldRequestDto.WorkerId,
                Line = workersFieldRequestDto.Line,
                FieldName = workersFieldRequestDto.FieldName,
                FieldValue = workersFieldRequestDto.FieldValue,
                Notes = workersFieldRequestDto.Notes,
                HideOnLayout = workersFieldRequestDto.HideOnLayout,
                Tbcreated = workersFieldRequestDto.Tbcreated,
                Tbmodified = workersFieldRequestDto.Tbmodified,
                TbcreatedId = workersFieldRequestDto.TbcreatedId,
                TbmodifiedId = workersFieldRequestDto.TbmodifiedId
            };
        }

        public static WorkersFieldRequestDto ToWorkersFieldRequestDto(this RmWorkersField workersField)
        {
            return new WorkersFieldRequestDto
            {
                WorkerId = workersField.WorkerId,
                Line = workersField.Line,
                FieldName = workersField.FieldName,
                FieldValue = workersField.FieldValue
            };
        }
    }
}
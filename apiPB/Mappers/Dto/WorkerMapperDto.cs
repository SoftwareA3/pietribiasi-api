using apiPB.Dto.Request;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Dto
{
    // Classe che mappa il Model in Dto
    public static class WorkerMapperDto
    {
        // Models
        public static WorkerDto ToWorkerDto(this VwApiWorker workerModel)
        {
            return new WorkerDto
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

        public static WorkersFieldDto ToWorkersFieldDto(this RmWorkersField workersField)
        {
            return new WorkersFieldDto
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

        // Request
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

        public static WorkerIdAndPasswordRequestDto ToWorkerIdAndPasswordRequestDto(this WorkerDto worker)
        {
            return new WorkerIdAndPasswordRequestDto
            {
                WorkerId = worker.WorkerId,
                Password = worker.Password ?? string.Empty
            };
        }
    }
}
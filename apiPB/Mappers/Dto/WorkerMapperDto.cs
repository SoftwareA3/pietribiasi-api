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

        public static WorkersRequestDto ToWorkersRequestDtoFromDto(this WorkerDto workerDto)
        {
            return new WorkersRequestDto
            {
                WorkerId = workerDto.WorkerId,
                Name = workerDto.Name,
                LastName = workerDto.LastName,
                Pin = workerDto.Pin,
                Password = workerDto.Password,
                TipoUtente = workerDto.TipoUtente,
                StorageVersamenti = workerDto.StorageVersamenti,
                Storage = workerDto.Storage,
                LastLogin = workerDto.LastLogin
            };
        }
    }
}
using apiPB.Mappers.Dto;
using apiPB.Services.Request.Abstraction;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Validation;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Request.Implementation
{
    public class WorkersRequestService : IWorkersRequestService
    {
        // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public WorkersRequestService(IWorkerRepository workerRepository, IMapper mapper)
        {
            _workerRepository = workerRepository;
            _mapper = mapper;
        }

        public IEnumerable<WorkerDto> GetWorkers()
        {
            return _workerRepository.GetWorkers()
            .Select(w => w.ToWorkerDto());
        }

        public WorkerDto? GetWorkerByPassword(WorkersRequestDto request)
        {
            request.ValidatePasswordRequestDto();
            var filter = _mapper.Map<WorkersFilter>(request);
            var worker = _workerRepository.GetWorkerByPassword(filter);
            return worker != null ? worker.ToWorkerDto() : null;
        }

        public Task CallStoredProcedure(WorkersFieldRequestDto request)
        {
            request.ValidateIdAndValueRequestDto();
            var filter = _mapper.Map<WorkersFieldFilter>(request);
            return _workerRepository.CallStoredProcedure(filter);
        }

        public IEnumerable<WorkersFieldDto> GetWorkersFieldsById(WorkersFieldRequestDto request)
        {
            request.ValidateIdRequestDto();
            var filter = _mapper.Map<WorkersFieldFilter>(request);
            return _workerRepository.GetWorkersFieldsById(filter)
            .Select(w => w.ToWorkersFieldDto());
        }

        public WorkersFieldDto? GetLastWorkerFieldLine(WorkersRequestDto request)
        {
            request.ValidateIdRequestDto();
            var filter = _mapper.Map<WorkersFieldFilter>(request);
            var worker = _workerRepository.GetLastWorkerFieldLine(filter);
            return worker != null ? worker.ToWorkersFieldDto() : null;
        }

        public async Task<WorkersFieldDto?> UpdateOrCreateLastLogin(WorkersRequestDto request)
        {
            request.ValidatePasswordRequestDto();
            var workerDto = GetWorkerByPassword(request);
            if (workerDto == null)
            {
                return null;
            }
            // Passare il dto al dto di richiesta
            var workerRequestDto = workerDto.ToWorkersRequestDtoFromDto();
            var filter = _mapper.Map<WorkersFilter>(workerDto);
            await _workerRepository.CreateOrUpdateLastLogin(filter);
            return GetLastWorkerFieldLine(workerRequestDto);
        }

        public WorkerDto? LoginWithPassword(WorkersRequestDto request)
        {
            request.ValidatePasswordRequestDto();
            var workerDto = GetWorkerByPassword(request);
            if (workerDto == null)
            {
                return null;
            }
            UpdateOrCreateLastLogin(request).Wait();
            return workerDto;
        }

        public WorkerDto? GetWorkerByIdAndPassword(WorkersRequestDto request)
        {
            var filter = _mapper.Map<WorkersFilter>(request);
            var worker = _workerRepository.GetWorkerByIdAndPassword(filter);
            if (worker == null)
            {
                return null;
            }
            UpdateOrCreateLastLogin(new WorkersRequestDto { Password = request.Password }).Wait();
            return worker.ToWorkerDto();
        }
    }
}
using apiPB.Mappers.Dto;
using apiPB.Services.Abstraction;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Implementation
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

        public WorkerDto GetWorkerByPassword(PasswordWorkersRequestDto request)
        {
            var filter = _mapper.Map<PasswordWorkersRequestFilter>(request);
            var worker = _workerRepository.GetWorkerByPassword(filter);
            return worker?.ToWorkerDto() ?? throw new ArgumentNullException(nameof(worker), "Nessun lavoratore trovato con la password fornita.");
        }

        public Task CallStoredProcedure(WorkerDto request)
        {
            var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
            return _workerRepository.CallStoredProcedure(filter);
        }

        public IEnumerable<WorkersFieldDto> GetWorkersFieldsById(WorkersFieldRequestDto request)
        {
            var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
            return _workerRepository.GetWorkersFieldsById(filter)
            .Select(w => w.ToWorkersFieldDto());
        }

        public WorkersFieldDto? GetLastWorkerFieldLine(WorkerDto request)
        {
            var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
            var worker = _workerRepository.GetLastWorkerFieldLine(filter);
            return worker?.ToWorkersFieldDto();
        }

        public async Task<WorkersFieldDto?> UpdateOrCreateLastLogin(PasswordWorkersRequestDto request)
        {
            var workerDto = GetWorkerByPassword(request);
            var filter = _mapper.Map<PasswordWorkersRequestFilter>(workerDto);
            await _workerRepository.CreateOrUpdateLastLogin(filter);
            return GetLastWorkerFieldLine(workerDto);
        }

        public WorkerDto? LoginWithPassword(PasswordWorkersRequestDto request)
        {
            var workerDto = GetWorkerByPassword(request);
            UpdateOrCreateLastLogin(request).Wait();
            return workerDto;
        }

        public WorkerDto? GetWorkerByIdAndPassword(WorkerIdAndPasswordRequestDto request)
        {
            var filter = _mapper.Map<WorkerIdAndPasswordFilter>(request);
            var worker = _workerRepository.GetWorkerByIdAndPassword(filter);
            UpdateOrCreateLastLogin(new PasswordWorkersRequestDto { Password = request.Password }).Wait();
            return worker.ToWorkerDto();
        }
    }
}
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
            try
            {
                return _workerRepository.GetWorkers()
                .Select(w => w.ToWorkerDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public WorkerDto GetWorkerByPassword(PasswordWorkersRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<PasswordWorkersRequestFilter>(request);
                var worker = _workerRepository.GetWorkerByPassword(filter);
                return worker?.ToWorkerDto() ?? throw new ArgumentNullException("Worker non trovato con la password fornita");
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public Task CallStoredProcedure(WorkerDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
                return _workerRepository.CallStoredProcedure(filter);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public IEnumerable<WorkersFieldDto> GetWorkersFieldsById(WorkersFieldRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
                return _workerRepository.GetWorkersFieldsById(filter)
                .Select(w => w.ToWorkersFieldDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public WorkersFieldDto? GetLastWorkerFieldLine(WorkerDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdAndValueRequestFilter>(request);
                var worker = _workerRepository.GetLastWorkerFieldLine(filter);
                return worker?.ToWorkersFieldDto() ?? throw new ArgumentNullException("Ultimo campo del lavoratore non trovato");
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public async Task<WorkersFieldDto?> UpdateOrCreateLastLogin(PasswordWorkersRequestDto request)
        {
            try
            {
                var workerDto = GetWorkerByPassword(request) ?? throw new ArgumentNullException("Worker non trovato con la password fornita");
                var filter = _mapper.Map<PasswordWorkersRequestFilter>(workerDto);
                await _workerRepository.CreateOrUpdateLastLogin(filter);
                return GetLastWorkerFieldLine(workerDto);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public WorkerDto? LoginWithPassword(PasswordWorkersRequestDto request)
        {
            try
            {
                var workerDto = GetWorkerByPassword(request) ?? throw new ArgumentNullException("Worker non trovato con la password fornita");
                UpdateOrCreateLastLogin(request).Wait();
                return workerDto;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }

        public WorkerDto? GetWorkerByIdAndPassword(WorkerIdAndPasswordRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdAndPasswordFilter>(request);
                var worker = _workerRepository.GetWorkerByIdAndPassword(filter) ?? throw new ArgumentNullException("Worker non trovato con l'ID e la password forniti");
                UpdateOrCreateLastLogin(new PasswordWorkersRequestDto { Password = request.Password }).Wait();
                return worker.ToWorkerDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in WorkersRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service WorkersRequestService", ex);
            }
        }
    }
}
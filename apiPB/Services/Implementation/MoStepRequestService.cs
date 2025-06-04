using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Abstraction;

namespace apiPB.Services.Implementation
{
    public class MoStepRequestService : IMoStepRequestService
    {
        private readonly IMapper _mapper;
        private readonly IMoStepRepository _repository;
        public MoStepRequestService(IMapper mapper, IMoStepRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<MostepDto> GetMostepWithJob(JobRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<JobFilter>(request);
                return _repository.GetMostepWithJob(filter)
                .Select(m => m.ToMostepDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MoStepRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MoStepRequestService", ex);
            }
        }

        public IEnumerable<MostepDto> GetMostepWithMono(MonoRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<MonoFilter>(request);
                return _repository.GetMostepWithMono(filter)
                .Select(m => m.ToMostepDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MoStepRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MoStepRequestService", ex);
            }
        }

        public IEnumerable<MostepDto> GetMostepWithOperation(OperationRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<OperationFilter>(request);
                return _repository.GetMostepWithOperation(filter)
                .Select(m => m.ToMostepDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MoStepRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MoStepRequestService", ex);
            }
        }
    }
}
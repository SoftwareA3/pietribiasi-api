using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Abstraction;

namespace apiPB.Services.Implementation
{
    public class MostepsMocomponentRequestService : IMostepsMocomponentRequestService
    {
        private readonly IMostepsMocomponentRepository _repository;
        private readonly IMapper _mapper;
        public MostepsMocomponentRequestService(IMostepsMocomponentRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentJobDistinct(JobRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<JobFilter>(request);
                return _repository.GetMostepsMocomponentJob(filter)
                .Select(m => m.ToMostepsMocomponentDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MostepsMocomponentRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MostepsMocomponentRequestService", ex);
            }
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentMonoDistinct(MonoRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<MonoFilter>(request);
                return _repository.GetMostepsMocomponentMono(filter)
                .Select(m => m.ToMostepsMocomponentDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MostepsMocomponentRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MostepsMocomponentRequestService", ex);
            }
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentOperationDistinct(OperationRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<OperationFilter>(request);
                return _repository.GetMostepsMocomponentOperation(filter)
                .Select(m => m.ToMostepsMocomponentDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MostepsMocomponentRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MostepsMocomponentRequestService", ex);
            }
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentBarCodeDistinct(BarCodeRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<BarCodeFilter>(request);
                return _repository.GetMostepsMocomponentBarCode(filter)
                .Select(m => m.ToMostepsMocomponentDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in MostepsMocomponentRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service MostepsMocomponentRequestService", ex);
            }
        }
    }
}
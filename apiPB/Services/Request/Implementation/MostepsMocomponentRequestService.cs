using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Services.Request.Implementation
{
    public class MostepsMocomponentRequestService : IMostepsMocomponentRequestService
    {
        private readonly IMostepsMocomponentRepository _repository;
        private readonly IMoStepRequestService _moStepRequestService;
        private readonly IMapper _mapper;
        public MostepsMocomponentRequestService(IMostepsMocomponentRepository repository, IMoStepRequestService moStepRequestService, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _moStepRequestService = moStepRequestService;
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentJobDistinct(JobRequestDto request)
        {
            var filter = _mapper.Map<JobFilter>(request);
            return _repository.GetMostepsMocomponentJobDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentMonoDistinct(MonoRequestDto request)
        {
            var filter = _mapper.Map<MonoFilter>(request);
            return _repository.GetMostepsMocomponentMonoDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentOperationDistinct(OperationRequestDto request)
        {
            var filter = _mapper.Map<OperationFilter>(request);
            return _repository.GetMostepsMocomponentOperationDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentBarCodeDistinct(BarCodeRequestDto request)
        {
            var filter = _mapper.Map<BarCodeFilter>(request);
            return _repository.GetMostepsMocomponentBarCodeDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }
    }
}
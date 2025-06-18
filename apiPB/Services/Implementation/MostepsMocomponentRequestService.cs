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
            var filter = _mapper.Map<JobFilter>(request);
            return _repository.GetMostepsMocomponentJob(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentMonoDistinct(MonoRequestDto request)
        {
            var filter = _mapper.Map<MonoFilter>(request);
            return _repository.GetMostepsMocomponentMono(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentOperationDistinct(OperationRequestDto request)
        {
            var filter = _mapper.Map<OperationFilter>(request);
            return _repository.GetMostepsMocomponentOperation(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentBarCodeDistinct(BarCodeRequestDto request)
        {
            var filter = _mapper.Map<BarCodeFilter>(request);
            return _repository.GetMostepsMocomponentBarCode(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }
    }
}
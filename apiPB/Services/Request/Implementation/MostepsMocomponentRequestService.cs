using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;
using apiPB.Validation;

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

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentByMoId(MostepsMocomponentRequestDto request)
        {
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponent(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentDistinct(MostepsMocomponentRequestDto request)
        {
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentJobDistinct(MostepsMocomponentRequestDto request)
        {
            request.ValidateJobRequestDto();
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentJobDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentMonoDistinct(MostepsMocomponentRequestDto request)
        {
            request.ValidateMonoRequestDto();
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentMonoDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentOperationDistinct(MostepsMocomponentRequestDto request)
        {
            request.ValidateOperationRequestDto();
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentOperationDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentBarCodeDistinct(MostepsMocomponentRequestDto request)
        {
            request.ValidateBarCodeRequestDto();
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentBarCodeDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDtoFromModel());
        }
    }
}
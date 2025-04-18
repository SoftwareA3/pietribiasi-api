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
    public class MoStepRequestService : IMoStepRequestService
    {
        private readonly IMapper _mapper;
        private readonly IMoStepRepository _repository;
        public MoStepRequestService(IMapper mapper, IMoStepRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<MostepDto> GetMostepWithJob(MostepRequestDto request)
        {
            request.ValidateJobRequestDto();
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _repository.GetMostepWithJob(filter)
            .Select(m => m.ToMostepDtoFromModel());
        }

        public IEnumerable<MostepDto> GetMostepWithMono(MostepRequestDto request)
        {
            request.ValidateMonoRequestDto();
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _repository.GetMostepWithMono(filter)
            .Select(m => m.ToMostepDtoFromModel());
        }

        public IEnumerable<MostepDto> GetMostepWithOperation(MostepRequestDto request)
        {
            request.ValidateOperationRequestDto();
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _repository.GetMostepWithOperation(filter)
            .Select(m => m.ToMostepDtoFromModel());
        }
    }
}
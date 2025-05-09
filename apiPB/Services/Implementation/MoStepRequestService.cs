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
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<JobFilter>(request);
            return _repository.GetMostepWithJob(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithMono(MonoRequestDto request)
        {
            var filter = _mapper.Map<MonoFilter>(request);
            return _repository.GetMostepWithMono(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithOperation(OperationRequestDto request)
        {
            var filter = _mapper.Map<OperationFilter>(request);
            return _repository.GetMostepWithOperation(filter)
            .Select(m => m.ToMostepDto());
        }
    }
}
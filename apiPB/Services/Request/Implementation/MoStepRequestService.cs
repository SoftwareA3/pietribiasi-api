using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;

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

        public IEnumerable<MostepDto> GetMostepWithJob(MostepJobRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepJobFilter>(request);
            return _repository.GetMostepWithJob(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithMono(MostepMonoRequestDto request)
        {
            var filter = _mapper.Map<MostepMonoFilter>(request);
            return _repository.GetMostepWithMono(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithOperation(MostepOperationRequestDto request)
        {
            var filter = _mapper.Map<MostepOperationFilter>(request);
            return _repository.GetMostepWithOperation(filter)
            .Select(m => m.ToMostepDto());
        }
    }
}
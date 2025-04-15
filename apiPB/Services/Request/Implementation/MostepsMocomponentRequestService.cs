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

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentByMoId(MostepsMocomponentRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponent(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentDistinct(MostepsMocomponentRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _repository.GetMostepsMocomponentDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentForRegOre(MostepRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var moStepDto = _moStepRequestService.GetMostepByMoId(request)
            .FirstOrDefault();
            var MostepsMocomponentDto = _mapper.Map<MostepsMocomponentRequestDto>(moStepDto);
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(MostepsMocomponentDto);
            return GetMostepsMocomponentDistinct(MostepsMocomponentDto);
        }
    }
}
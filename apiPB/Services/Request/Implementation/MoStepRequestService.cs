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

        public IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _repository.GetMostep(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithOdp(MostepOdpRequestDto request)
        {
            var filter = _mapper.Map<MostepOdpRequestFilter>(request);
            return _repository.GetMostepWithOdp(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithLavorazione(MostepLavorazioniRequestDto request)
        {
            var filter = _mapper.Map<MostepLavorazioniRequestFilter>(request);
            return _repository.GetMostepWithLavorazione(filter)
            .Select(m => m.ToMostepDto());
        }
    }
}
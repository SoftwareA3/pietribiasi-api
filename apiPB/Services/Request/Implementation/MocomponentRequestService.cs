using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Services.Request.Implementation
{
    // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
    public class MocomponentRequestService : IMocomponentRequestService
    {
        private readonly IVwApiMocomponentRepository _moComponentRepository;
        private readonly IMapper _mapper;

        public MocomponentRequestService(IVwApiMocomponentRepository moComponentRepository, IMapper mapper)
        {
            _moComponentRepository = moComponentRepository;
            _mapper = mapper;
        }

        public IEnumerable<VwApiMocomponentDto> GetVwApiMocomponent(VwApiMocomponentRequestDto request)
        {
            var filter = _mapper.Map<VwApiMocomponentRequestFilter>(request);
            return _moComponentRepository.GetVwApiMocomponent(filter)
            .Select(m => m.ToVwApiMocomponentDto());
        }
    }
}
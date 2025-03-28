using apiPB.Services.Request.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Request.Implementation
{
    public class MostepRequestService : IMostepRequestService
    {
        // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
        private readonly IVwApiMostepRepository _vwApiMostepRepository;
        private readonly IMapper _mapper;

        public MostepRequestService(IVwApiMostepRepository vwApiMostepRepository, IMapper mapper)
        {
            _vwApiMostepRepository = vwApiMostepRepository;
            _mapper = mapper;
        }

        public IEnumerable<VwApiMostepDto> GetMostepByMoId(VwApiMostepRequestDto request)
        {
            var filter = _mapper.Map<VwApiMostepRequestFilter>(request);
            return _vwApiMostepRepository.GetVwApiMostep(filter)
            .Select(m => m.ToVwApiMostepDto());
        }
    }
}
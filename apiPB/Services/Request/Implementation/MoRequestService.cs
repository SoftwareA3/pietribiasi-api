using apiPB.Services.Request.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Request.Implementation
{
    // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
    public class MoRequestService : IMoRequestService
    {
        private readonly IVwApiMoRepository _vwApiMoRepository;
        private readonly IMapper _mapper;

        public MoRequestService(IVwApiMoRepository vwApiMoRepository, IMapper mapper)
        {
            _vwApiMoRepository = vwApiMoRepository;
            _mapper = mapper;
        }

        public IEnumerable<VwApiMoDto> GetVwApiMo(VwApiMoRequestDto request)
        {
            var filter = _mapper.Map<VwApiMoRequestFilter>(request);
            return _vwApiMoRepository.GetVwApiMo(filter)
            .Select(m => m.ToVwApiMoDto());
        }
    }
}
using apiPB.Services.Request.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Implementation
{
    public class MoStepsComponentRequestService : IMoStepsComponentRequestService
    {
        // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
        private readonly IVwApiMoStepsComponentRepository _vwApiMoStepsComponentRepository;
        private readonly IMapper _mapper;

        public MoStepsComponentRequestService(IVwApiMoStepsComponentRepository vwApiMoStepsComponentRepository, IMapper mapper)
        {
            _vwApiMoStepsComponentRepository = vwApiMoStepsComponentRepository;
            _mapper = mapper;
        }

        public IEnumerable<VwApiMoStepsComponentDto> GetMoStepsComponentByMoId(VwApiMoStepsComponentRequestDto request)
        {
            var filter = _mapper.Map<VwApiMoStepsComponentRequestFilter>(request);
            return _vwApiMoStepsComponentRepository.GetVwApiMoStepsComponent(filter)
            .Select(m => m.ToVwApiMoStepsComponentDto());
        }
    }
}
using apiPB.Services.Abstraction;
using apiPB.Dto.Models;
using apiPB.Mappers.Dto;
using AutoMapper;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Services.Implementation
{
    public class GiacenzeRequestService : IGiacenzeRequestService
    {
        private readonly IGiacenzeRepository _giacenzeRepository;
        private readonly IMapper _mapper;

        public GiacenzeRequestService(IGiacenzeRepository giacenzeRepository, IMapper mapper)
        {
            _giacenzeRepository = giacenzeRepository;
            _mapper = mapper;
        }

        public IEnumerable<GiacenzeDto> GetGiacenze()
        {
            var giacenze = _giacenzeRepository.GetGiacenze();
            var giacenzeDtoList = new List<GiacenzeDto>();

            foreach (var giacenza in giacenze)
            {
                var giacenzaDto = giacenza.ToGiacenzeDto();
                giacenzeDtoList.Add(giacenzaDto);
            }

            return giacenzeDtoList;
        }

        public GiacenzeDto GetGiacenzeByItem(ComponentRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Component))
            {
                throw new ArgumentException("Invalid request: Item cannot be null or empty.", nameof(request));
            }

            var componentFilter = _mapper.Map<ComponentFilter>(request);

            var giacenza = _giacenzeRepository.GetGiacenzaByItem(componentFilter);
            if (giacenza == null)
            {
                throw new ArgumentNullException("Item not found in giacenze.", nameof(request.Component));
            }

            return giacenza.ToGiacenzeDto();
        }
    }
}
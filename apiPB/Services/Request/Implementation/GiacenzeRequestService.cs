using apiPB.Services.Request.Abstraction;
using apiPB.Dto.Models;
using apiPB.Mappers.Dto;
using AutoMapper;
using apiPB.Repository.Abstraction;

namespace apiPB.Services.Request.Implementation
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
    }
}
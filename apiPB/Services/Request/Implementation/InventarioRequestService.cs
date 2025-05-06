using apiPB.Services.Request.Abstraction;
using AutoMapper;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Mappers.Dto;
using apiPB.Filters;
using apiPB.Repository.Abstraction;

namespace apiPB.Services.Request.Implementation
{
    public class InventarioRequestService : IInventarioRequestService
    {
        private readonly IInventarioRepository _repository;
        private readonly IMapper _mapper;

        public InventarioRequestService(IInventarioRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<InventarioDto> GetInventario()
        {
            return _repository.GetInventario()
            .Select(m => m.ToInventarioDto());
        }

        public IEnumerable<InventarioDto> PostInventarioList(IEnumerable<InventarioRequestDto> inventarioList)
        {
            var filterList = new List<InventarioFilter>();
            foreach (var request in inventarioList)
            {
                var filter = _mapper.Map<InventarioFilter>(request);
                filterList.Add(filter);
            }
            var result = _repository.PostInventarioList(filterList);

            var resultList = new List<InventarioDto>();
            foreach (var item in result)
            {
                var dto = item.ToInventarioDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public IEnumerable<InventarioDto> GetViewInventario(ViewInventarioRequestDto request)
        {
            var filter = _mapper.Map<ViewInventarioRequestFilter>(request);
            var result = _repository.GetViewInventario(filter);

            var resultList = new List<InventarioDto>();
            foreach (var item in result)
            {
                var dto = item.ToInventarioDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public InventarioDto PutViewInventario(ViewInventarioPutRequestDto request)
        {
            var filter = _mapper.Map<ViewInventarioPutFilter>(request);
            var result = _repository.PutViewInventario(filter);

            return result.ToInventarioDto();
        }
    }
}
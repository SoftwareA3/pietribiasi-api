using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Services.Request.Implementation
{
    public class RegOreRequestService : IRegOreRequestService
    {
        private readonly IRegOreRepository _repository;
        private readonly IMapper _mapper;

        public RegOreRequestService(IRegOreRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<RegOreDto> GetAppRegOre()
        {
            return _repository.GetAppRegOre()
            .Select(m => m.ToA3AppRegOreDto());
        }

        public IEnumerable<RegOreDto> PostAppRegOre(IEnumerable<RegOreRequestDto> requestList)
        {
            var filterList = new List<RegOreFilter>();
            foreach (var request in requestList)
            {
                var filter = _mapper.Map<RegOreFilter>(request);
                filterList.Add(filter);
            }
            var result = _repository.PostRegOreList(filterList);

            var resultList = new List<RegOreDto>();
            foreach (var item in result)
            {
                var dto = item.ToA3AppRegOreDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public IEnumerable<RegOreDto> GetAppViewOre(ViewOreRequestDto request)
        {
            var filter = _mapper.Map<ViewOreRequestFilter>(request);
            var result = _repository.GetAppViewOre(filter);

            var resultList = new List<RegOreDto>();
            foreach (var item in result)
            {
                var dto = item.ToA3AppRegOreDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public RegOreDto? PutAppViewOre(ViewOrePutRequestDto request)
        {
            var filter = _mapper.Map<ViewOrePutFilter>(request);
            var result = _repository.PutAppViewOre(filter);
            if (result != null)
            {
                return result.ToA3AppRegOreDto();
            }
            else
            {
                return null;
            }
        }

        public RegOreDto DeleteRegOreId(ViewOreDeleteRequestDto request)
        {
            var filter = _mapper.Map<ViewOreDeleteRequestFilter>(request);
            var result = _repository.DeleteRegOreId(filter);
            if (result != null)
            {
                return result.ToA3AppRegOreDto();
            }
            else
            {
                throw new InvalidOperationException("Record not found.");
            }
        }
    }
}
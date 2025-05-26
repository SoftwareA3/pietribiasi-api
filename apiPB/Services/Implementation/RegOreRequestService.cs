using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Abstraction;

namespace apiPB.Services.Implementation
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

        public RegOreDto? DeleteRegOreId(ViewOreDeleteRequestDto request)
        {
            var filter = _mapper.Map<ViewOreDeleteRequestFilter>(request);
            var result = _repository.DeleteRegOreId(filter);
            if (result != null)
            {
                return result.ToA3AppRegOreDto();
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<RegOreDto> GetNotImportedAppRegOre()
        {
            return _repository.GetNotImportedRegOre()
            .Select(m => m.ToA3AppRegOreDto());
        }

        public IEnumerable<RegOreDto> UpdateRegOreImported(WorkerIdSyncRequestDto request)
        {
            if (request == null)
            {
                return null;
            }

            var filter = _mapper.Map<WorkerIdSyncFilter>(request);
            var result = _repository.UpdateRegOreImported(filter);

            return result
            .Select(m => m.ToA3AppRegOreDto());
        }

        public IEnumerable<RegOreDto> GetNotImportedAppRegOreByFilter(ViewOreRequestDto request)
        {
            if (request == null)
            {
                return null;
            }

            var filter = _mapper.Map<ViewOreRequestFilter>(request);
            var result = _repository.GetNotImportedAppRegOreByFilter(filter);

            return result
            .Select(m => m.ToA3AppRegOreDto());
        }

        public IEnumerable<RegOreDto> UpdateImportedById(UpdateImportedIdRequestDto request)
        {
            if (request == null)
            {
                return null;
            }

            var filter = _mapper.Map<UpdateImportedIdFilter>(request);
            var result = _repository.UpdateImportedById(filter);

            return result
            .Select(m => m.ToA3AppRegOreDto());
        }
    }
}
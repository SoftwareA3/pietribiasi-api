using apiPB.Services.Abstraction;
using AutoMapper;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Mappers.Dto;
using apiPB.Filters;

namespace apiPB.Services.Implementation
{
    public class PrelMatRequestService : IPrelMatRequestService
    {
        private readonly IPrelMatRepository _repository;
        private readonly IMapper _mapper;

        public PrelMatRequestService(IPrelMatRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<PrelMatDto> GetAppPrelMat()
        {
            return _repository.GetAppPrelMat()
            .Select(m => m.ToDtoPrelMatDto());
        }

        public IEnumerable<PrelMatDto> PostPrelMatList(IEnumerable<PrelMatRequestDto> requestList)
        {
            var filterList = new List<PrelMatFilter>();
            foreach (var request in requestList)
            {
                var filter = _mapper.Map<PrelMatFilter>(request);
                filterList.Add(filter);
            }
            var result = _repository.PostPrelMatList(filterList);

            var resultList = new List<PrelMatDto>();
            foreach (var item in result)
            {
                var dto = item.ToDtoPrelMatDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public IEnumerable<PrelMatDto> GetViewPrelMatList(ViewPrelMatRequestDto request)
        {
            var filter = _mapper.Map<ViewPrelMatRequestFilter>(request);
            var result = _repository.GetViewPrelMat(filter);

            var resultList = new List<PrelMatDto>();
            foreach (var item in result)
            {
                var dto = item.ToDtoPrelMatDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public PrelMatDto? PutViewPrelMat(ViewPrelMatPutRequestDto request)
        {
            var filter = _mapper.Map<ViewPrelMatPutFilter>(request);
            var result = _repository.PutViewPrelMat(filter);
            return result?.ToDtoPrelMatDto();
        }

        public PrelMatDto? DeletePrelMatId(ViewPrelMatDeleteRequestDto request)
        {
            var filter = _mapper.Map<ViewPrelMatDeleteFilter>(request);
            var result = _repository.DeletePrelMatId(filter);
            return result?.ToDtoPrelMatDto();
        }

        public IEnumerable<PrelMatDto> GetPrelMatWithComponent(ComponentRequestDto? request)
        {
            if (request == null)
            {
                return null;
            }
            var filter = _mapper.Map<ComponentFilter>(request);
            var result = _repository.GetPrelMatWithComponent(filter);

            if (result == null)
            {
                return null;
            }

            var resultList = new List<PrelMatDto>();
            foreach (var item in result)
            {
                var dto = item.ToDtoPrelMatDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public IEnumerable<PrelMatDto> GetNotImportedPrelMat()
        {
            return _repository.GetNotImportedPrelMat()
            .Select(m => m.ToDtoPrelMatDto());
        }

        public IEnumerable<PrelMatDto> UpdatePrelMatImported(WorkerIdSyncRequestDto request)
        {
            if (request == null)
            {
                return null;
            }

            var filter = _mapper.Map<WorkerIdSyncFilter>(request);
            var result = _repository.UpdatePrelMatImported(filter);

            return result
            .Select(m => m.ToDtoPrelMatDto());
        }
    }
}
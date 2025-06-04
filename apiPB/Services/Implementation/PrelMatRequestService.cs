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
            try
            {
                return _repository.GetAppPrelMat()
                .Select(m => m.ToDtoPrelMatDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in PrelMatRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service PrelMatRequestService", ex);
            }
        }

        public IEnumerable<PrelMatDto> PostPrelMatList(IEnumerable<PrelMatRequestDto> requestList)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in PrelMatRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service PrelMatRequestService", ex);
            }
        }

        public IEnumerable<PrelMatDto> GetViewPrelMatList(ViewPrelMatRequestDto request)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetViewPrelMatList", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetViewPrelMatList", ex);
            }
        }

        public PrelMatDto PutViewPrelMat(ViewPrelMatPutRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewPrelMatPutFilter>(request);
                var result = _repository.PutViewPrelMat(filter);
                return result.ToDtoPrelMatDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in PutViewPrelMat", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service PutViewPrelMat", ex);
            }
        }

        public PrelMatDto DeletePrelMatId(ViewPrelMatDeleteRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewPrelMatDeleteFilter>(request);
                var result = _repository.DeletePrelMatId(filter);
                return result.ToDtoPrelMatDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in DeletePrelMatId", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service DeletePrelMatId", ex);
            }
        }

        public IEnumerable<PrelMatDto> GetPrelMatWithComponent(ComponentRequestDto? request)
        {
            try
            {
                var filter = _mapper.Map<ComponentFilter>(request);
                var result = _repository.GetPrelMatWithComponent(filter);

                if (result == null)
                {
                    throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetPrelMatWithComponent");
                }

                var resultList = new List<PrelMatDto>();
                foreach (var item in result)
                {
                    var dto = item.ToDtoPrelMatDto();
                    resultList.Add(dto);
                }

                return resultList;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetPrelMatWithComponent", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetPrelMatWithComponent", ex);
            }
        }

        public IEnumerable<PrelMatDto> GetNotImportedPrelMat()
        {
            try
            {
                return _repository.GetNotImportedPrelMat()
                .Select(m => m.ToDtoPrelMatDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetNotImportedPrelMat", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetNotImportedPrelMat", ex);
            }
        }

        public IEnumerable<PrelMatDto> UpdatePrelMatImported(WorkerIdSyncRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdSyncFilter>(request);
                var result = _repository.UpdatePrelMatImported(filter);

                return result
                .Select(m => m.ToDtoPrelMatDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in UpdatePrelMatImported", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service UpdatePrelMatImported", ex);
            }
        }

        public IEnumerable<PrelMatDto> GetNotImportedAppPrelMatByFilter(ViewPrelMatRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewPrelMatRequestFilter>(request);
                var result = _repository.GetNotImportedAppPrelMatByFilter(filter);

                return result
                .Select(m => m.ToDtoPrelMatDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetNotImportedAppPrelMatByFilter", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetNotImportedAppPrelMatByFilter", ex);
            }
        }

        public IEnumerable<PrelMatDto> UpdateImportedById(UpdateImportedIdRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<UpdateImportedIdFilter>(request);
                var result = _repository.UpdateImportedById(filter);

                return result
                .Select(m => m.ToDtoPrelMatDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in UpdateImportedById", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service UpdateImportedById", ex);
            }
        }
    }
}
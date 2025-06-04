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
            try
            {
                return _repository.GetAppRegOre()
                .Select(m => m.ToA3AppRegOreDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in RegOreRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service RegOreRequestService", ex);
            }
        }

        public IEnumerable<RegOreDto> PostAppRegOre(IEnumerable<RegOreRequestDto> requestList)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in RegOreRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service RegOreRequestService", ex);
            }
        }

        public IEnumerable<RegOreDto> GetAppViewOre(ViewOreRequestDto request)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in RegOreRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service RegOreRequestService", ex);
            }
        }

        public RegOreDto? PutAppViewOre(ViewOrePutRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewOrePutFilter>(request);
                var result = _repository.PutAppViewOre(filter) ?? throw new Exception("Nessun risultato per PutAppViewOre in RegOreRequestService");
                return result.ToA3AppRegOreDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in RegOreRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service RegOreRequestService", ex);
            }
        }

        public RegOreDto? DeleteRegOreId(ViewOreDeleteRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewOreDeleteRequestFilter>(request);
                var result = _repository.DeleteRegOreId(filter);
                if (result == null)
                {
                    throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in DeleteRegOreId");
                }
                return result.ToA3AppRegOreDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in DeleteRegOreId", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service DeleteRegOreId", ex);
            }
        }

        public IEnumerable<RegOreDto> GetNotImportedAppRegOre()
        {
            try
            {
                return _repository.GetNotImportedRegOre()
                .Select(m => m.ToA3AppRegOreDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetNotImportedAppRegOre", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetNotImportedAppRegOre", ex);
            }
        }

        public IEnumerable<RegOreDto> UpdateRegOreImported(WorkerIdSyncRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdSyncFilter>(request);
                var result = _repository.UpdateRegOreImported(filter);

                return result
                .Select(m => m.ToA3AppRegOreDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in UpdateRegOreImported", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service UpdateRegOreImported", ex);
            }
        }

        public IEnumerable<RegOreDto> GetNotImportedAppRegOreByFilter(ViewOreRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewOreRequestFilter>(request);
                var result = _repository.GetNotImportedAppRegOreByFilter(filter);

                return result
                .Select(m => m.ToA3AppRegOreDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in GetNotImportedAppRegOreByFilter", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service GetNotImportedAppRegOreByFilter", ex);
            }
        }

        public IEnumerable<RegOreDto> UpdateImportedById(UpdateImportedIdRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<UpdateImportedIdFilter>(request);
                var result = _repository.UpdateImportedById(filter);

                return result
                .Select(m => m.ToA3AppRegOreDto());
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
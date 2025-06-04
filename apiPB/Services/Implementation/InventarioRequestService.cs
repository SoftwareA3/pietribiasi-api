using apiPB.Services.Abstraction;
using AutoMapper;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Mappers.Dto;
using apiPB.Filters;
using apiPB.Repository.Abstraction;

namespace apiPB.Services.Implementation
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
            try
            {
                return _repository.GetInventario()
                .Select(m => m.ToInventarioDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public IEnumerable<InventarioDto> PostInventarioList(IEnumerable<InventarioRequestDto> inventarioList)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public IEnumerable<InventarioDto> GetViewInventario(ViewInventarioRequestDto request)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public InventarioDto PutViewInventario(ViewInventarioPutRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewInventarioPutFilter>(request);
                var result = _repository.PutViewInventario(filter);

                return result.ToInventarioDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public IEnumerable<InventarioDto> GetNotImportedInventario()
        {
            try
            {
                return _repository.GetNotImportedInventario()
                .Select(m => m.ToInventarioDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public IEnumerable<InventarioDto> UpdateInventarioImported(WorkerIdSyncRequestDto? request)
        {
            try
            {
                var filter = _mapper.Map<WorkerIdSyncFilter>(request);
                var result = _repository.UpdateInventarioImported(filter);

                return result
                .Select(m => m.ToInventarioDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public IEnumerable<InventarioDto> GetNotImportedAppInventarioByFilter(ViewInventarioRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<ViewInventarioRequestFilter>(request);
                var result = _repository.GetNotImportedAppInventarioByFilter(filter);

                return result
                .Select(m => m.ToInventarioDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }

        public InventarioDto UpdateImportedById(UpdateImportedIdRequestDto request)
        {
            try
            {
                var filter = _mapper.Map<UpdateImportedIdFilter>(request);
                var result = _repository.UpdateImportedById(filter) ?? throw new Exception("Nessun risultato trovato per l'ID specificato");

                return result.ToInventarioDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in InventarioRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service InventarioRequestService", ex);
            }
        }
    }
}
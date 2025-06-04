using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;
using apiPB.Services.Abstraction;

namespace apiPB.Services.Implementation
{
    public class ActionMessageRequestService : IActionMessageRequestService
    {
        private readonly IActionMessageRepository _repository;
        private readonly IMapper _mapper;

        public ActionMessageRequestService(IActionMessageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public ActionMessageListDto GetActionMessagesByFilter(ImportedLogMessageDto request)
        {
            try
            {
                var filter = _mapper.Map<ImportedLogMessageFilter>(request);
                var messageList = _repository.GetActionMessagesByFilter(filter);

                return messageList.ToList().ToOmActionMessageDto();
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in ActionMessageRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service ActionMessageRequestService", ex);
            }
        }
    }
}
using apiPB.Mappers.Dto;
using apiPB.Services.Request.Abstraction;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using AutoMapper;

namespace apiPB.Services.Request.Implementation
{
    public class PasswordWorkersRequestService : IPasswordWorkersRequestService
    {
        // Mappa automaticamente il Dto nei Filter, chiama il repository per eseguire la query e mappa il risultato in un Dto
        private readonly IVwApiWorkerRepository _vwApiWorkerRepository;
        private readonly IMapper _mapper;

        public PasswordWorkersRequestService(IVwApiWorkerRepository vwApiWorkerRepository, IMapper mapper)
        {
            _vwApiWorkerRepository = vwApiWorkerRepository;
            _mapper = mapper;
        }

        public VwApiWorkerDto? GetWorkerByPassword(PasswordWorkersRequestDto request)
        {
            var filter = _mapper.Map<PasswordWorkersRequestFilter>(request);
            var worker = _vwApiWorkerRepository.GetVwApiWorkerByPassword(filter);
            return worker != null ? worker.ToWorkerDto() : null;
        }
    }
}
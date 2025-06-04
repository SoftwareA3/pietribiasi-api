using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Abstraction;

namespace apiPB.Services.Implementation
{
    public class JobRequestService : IJobRequestService
    {
        private readonly IJobRepository _repository;
        private readonly IMapper _mapper;

        public JobRequestService(IJobRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<JobDto> GetJobs()
        {
            try
            {
                return _repository.GetJobs()
                .Select(j => j.ToJobDto());
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in JobRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service JobRequestService", ex);
            }
                
        }
    }
}
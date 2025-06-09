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
            return _repository.GetJobs()
            .Select(j => j.ToJobDto());
        }
    }
}
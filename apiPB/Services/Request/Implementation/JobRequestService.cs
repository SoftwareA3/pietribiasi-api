using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Dto.Models;
using AutoMapper;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Services.Request.Implementation
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
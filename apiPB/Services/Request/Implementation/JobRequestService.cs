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
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        public JobRequestService(IJobRepository jobRepository, IMapper mapper)
        {
            _mapper = mapper;
            _jobRepository = jobRepository;
        }

        public IEnumerable<JobDto> GetJobs()
        {
            return _jobRepository.GetJobs()
            .Select(j => j.ToJobDto());
        }

        public IEnumerable<MocomponentDto> GetMocomponent(MocomponentRequestDto request)
        {
            var filter = _mapper.Map<MocomponentRequestFilter>(request);
            return _jobRepository.GetMocomponent(filter)
            .Select(m => m.ToMocomponentDto());
        }

        public IEnumerable<MoDto> GetMo(MoRequestDto request)
        {
            var filter = _mapper.Map<MoRequestFilter>(request);
            return _jobRepository.GetMo(filter)
            .Select(m => m.ToMoDto());
        }

        public IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request)
        {
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _jobRepository.GetMostep(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MoStepsComponentDto> GetMoStepsComponentByMoId(MoStepsComponentRequestDto request)
        {
            var filter = _mapper.Map<MoStepsComponentRequestFilter>(request);
            return _jobRepository.GetMoStepsComponent(filter)
            .Select(m => m.ToMoStepsComponentDto());
        }

        public IEnumerable<MoStepsComponentDto> GetMoStepsComponentDistinct(MoStepsComponentRequestDto request)
        {
            var filter = _mapper.Map<MoStepsComponentRequestFilter>(request);
            return _jobRepository.GetMoStepsComponentDistinct(filter)
            .Select(m => m.ToMoStepsComponentDto());
        }

        public IEnumerable<MoStepsComponentDto> GetMoStepsComponentForRegOre(MostepRequestDto request)
        {
            var moStepDto = GetMostepByMoId(request)
            .FirstOrDefault();
            var moStepsComponentDto = _mapper.Map<MoStepsComponentRequestDto>(moStepDto);
            var filter = _mapper.Map<MoStepsComponentRequestFilter>(moStepsComponentDto);
            return GetMoStepsComponentDistinct(moStepsComponentDto);
        }
    }
}
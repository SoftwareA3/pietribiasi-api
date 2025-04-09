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

        public IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepRequestFilter>(request);
            return _jobRepository.GetMostep(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentByMoId(MostepsMocomponentRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _jobRepository.GetMostepsMocomponent(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentDistinct(MostepsMocomponentRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(request);
            return _jobRepository.GetMostepsMocomponentDistinct(filter)
            .Select(m => m.ToMostepsMocomponentDto());
        }

        public IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentForRegOre(MostepRequestDto request)
        {
            // FIXME controllo su filtro e su metodo di repository
            var moStepDto = GetMostepByMoId(request)
            .FirstOrDefault();
            var MostepsMocomponentDto = _mapper.Map<MostepsMocomponentRequestDto>(moStepDto);
            var filter = _mapper.Map<MostepsMocomponentRequestFilter>(MostepsMocomponentDto);
            return GetMostepsMocomponentDistinct(MostepsMocomponentDto);
        }
    }
}
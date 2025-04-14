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

        public IEnumerable<MostepDto> GetMostepWithOdp(MostepOdpRequestDto request)
        {
            var filter = _mapper.Map<MostepOdpRequestFilter>(request);
            return _jobRepository.GetMostepWithOdp(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<MostepDto> GetMostepWithLavorazione(MostepLavorazioniRequestDto request)
        {
            var filter = _mapper.Map<MostepLavorazioniRequestFilter>(request);
            return _jobRepository.GetMostepWithLavorazione(filter)
            .Select(m => m.ToMostepDto());
        }

        public IEnumerable<A3AppRegOreDto> PostAppRegOre(IEnumerable<A3AppRegOreRequestDto> requestList)
        {
            var filterList = new List<A3AppRegOreFilter>();
            foreach (var request in requestList)
            {
                var filter = _mapper.Map<A3AppRegOreFilter>(request);
                filterList.Add(filter);
            }
            var result = _jobRepository.PostRegOreList(filterList);

            var resultList = new List<A3AppRegOreDto>();
            foreach (var item in result)
            {
                var dto = item.ToA3AppRegOreDto();
                resultList.Add(dto);
            }

            return resultList;
        }

        public IEnumerable<A3AppRegOreDto> GetAppViewOre(A3AppViewOreRequestDto request)
        {
            var filter = _mapper.Map<A3AppViewOreRequestFilter>(request);
            var result = _jobRepository.GetAppViewOre(filter);

            var resultList = new List<A3AppRegOreDto>();
            foreach (var item in result)
            {
                var dto = item.ToA3AppRegOreDto();
                resultList.Add(dto);
            }

            return resultList;
        }
    }
}
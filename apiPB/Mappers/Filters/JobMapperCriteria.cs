using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filter
{
    public class JobMapperFilters : Profile
    {
        public JobMapperFilters()
        {
            //Mappa automaticamente i Dto nei Filtri
            CreateMap<VwApiMoRequestDto, VwApiMoRequestFilter>();
            CreateMap<VwApiMostepRequestDto, VwApiMostepRequestFilter>();
            CreateMap<VwApiMocomponentRequestDto, VwApiMocomponentRequestFilter>();
            CreateMap<VwApiMoStepsComponentRequestDto, VwApiMoStepsComponentRequestFilter>();
        }
    }
}
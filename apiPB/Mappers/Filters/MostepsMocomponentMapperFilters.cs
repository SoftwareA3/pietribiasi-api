using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filters
{
    public class MostepsMocomponentMapperFilters : Profile
    {
        public MostepsMocomponentMapperFilters()
        {
            CreateMap<MostepsMocomponentRequestDto, MostepsMocomponentRequestFilter>();
            CreateMap<MostepsMocomponentMonoRequestDto, MostepsMocomponentMonoFilter>();
            CreateMap<MostepsMocomponentJobRequestDto, MostepsMocomponentJobFilter>();
            CreateMap<MostepsMocomponentOperationRequestDto, MostepsMocomponentOperationFilter>();
            CreateMap<MostepsMocomponentBarCodeRequestDto, MostepsMocomponentBarCodeFilter>();
        }
    }
}
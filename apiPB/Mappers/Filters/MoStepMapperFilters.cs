using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Filters
{
    public class MoStepMapperFilters : Profile
    {
        public MoStepMapperFilters() 
        {
            CreateMap<MostepJobRequestDto, MostepJobFilter>();
            CreateMap<MostepOperationRequestDto, MostepOperationFilter>();
            CreateMap<MostepMonoRequestDto, MostepMonoFilter>();
        }
    }
}
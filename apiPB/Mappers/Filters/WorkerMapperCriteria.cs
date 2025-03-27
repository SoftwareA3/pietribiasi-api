using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filters
{
    public class WorkerMapperFilters : Profile
    {
        public WorkerMapperFilters()
        {
            CreateMap<PasswordWorkersRequestDto, PasswordWorkersRequestFilter>();
        }
    }
}
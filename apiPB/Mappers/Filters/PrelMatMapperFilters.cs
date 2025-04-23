using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Models;
using AutoMapper;

namespace apiPB.Mappers.Filters
{
    public class PrelMatMapperFilters : Profile
    {
        public PrelMatMapperFilters()
        {
            CreateMap<PrelMatRequestDto, PrelMatFilter>();
            CreateMap<PrelMatFilter, A3AppPrelMat>();
            CreateMap<ViewPrelMatRequestDto, ViewPrelMatRequestFilter>();
            CreateMap<ViewPrelMatPutRequestDto, ViewPrelMatPutFilter>();
            CreateMap<ViewPrelMatDeleteRequestDto, ViewPrelMatDeleteFilter>();
        }
    }
}
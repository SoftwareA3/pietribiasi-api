using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Models;

namespace apiPB.Mappers.Filters
{
    public class InventarioMapperFilters : Profile
    {
        public InventarioMapperFilters()
        {
            CreateMap<InventarioRequestDto, InventarioFilter>();
            CreateMap<InventarioFilter, A3AppInventario>();
        }
    }
}
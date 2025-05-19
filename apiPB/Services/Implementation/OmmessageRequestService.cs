using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;
using apiPB.Repository.Abstraction;
using apiPB.Services.Abstraction;
using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Mappers.Dto;
using AutoMapper;
using apiPB.Filters;

namespace apiPB.Services.Implementation
{
    public class OmmessageRequestService : IOmmessageRequestService
    {
        private readonly IOmmessageRepository _ommessageRepository;
        private readonly IMapper _mapper;
        public OmmessageRequestService(IOmmessageRepository ommessageRepository, IMapper mapper)
        {
            _ommessageRepository = ommessageRepository;
            _mapper = mapper;
        }

        public List<OmmessageInfoRequestDto> GetOmmessageGroupedByMoid(MoIdRequestDto moIdRequestDtos)
        {
            if (moIdRequestDtos == null)
            {
                return new List<OmmessageInfoRequestDto>();
            }

            var filter = _mapper.Map<MoIdFilter>(moIdRequestDtos);
            
            return _ommessageRepository.GetOmmessagesFilteredByMoId(filter).ToOmmessageInforequestDto();
        }
    }
}
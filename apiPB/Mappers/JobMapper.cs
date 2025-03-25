using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto;
using apiPB.Models;

namespace apiPB.Mappers
{
    public static class JobMapper
    {
        public static VwApiJobDto ToVwApiJobDto(this VwApiJob jobModel)
        {
            return new VwApiJobDto
            {
                Job = jobModel.Job ?? string.Empty,
                Description = jobModel.Description ?? string.Empty
            };
        }

        public static VwApiMoRequestDto ToVwApiMoRequestDto(this VwApiMo moModel)
        {
            return new VwApiMoRequestDto
            {
                Job = moModel.Job ?? string.Empty,
                RtgStep = moModel.RtgStep,
                Alternate = moModel.Alternate ?? string.Empty,
                AltRtgStep = moModel.AltRtgStep
            };
        }

        public static VwApiMostepRequestDto ToVwMostepRequestDto(this VwApiMostep mostepModel)
        {
            return new VwApiMostepRequestDto
            {
                Job = mostepModel.Job ?? string.Empty
            };
        }
    }
}
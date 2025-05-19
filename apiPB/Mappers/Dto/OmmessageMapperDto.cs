using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Dto
{
    public static class OmmessageMapperDto
    {
        public static OmmessageDto ToDtoOmmessageDto(this VwOmmessage request)
        {
            return new OmmessageDto
            {
                MessageId = request.MessageId,
                MessageType = request.MessageType,
                WorkerId = request.WorkerId,
                MessageDate = request.MessageDate,
                MessageText = request.MessageText,
                UserMessage = request.UserMessage,
                Expire = request.Expire,
                ExpirationDate = request.ExpirationDate,
                Moid = request.Moid,
                RtgStep = request.RtgStep,
                Alternate = request.Alternate,
                AltRtgStep = request.AltRtgStep,
                Tbcreated = request.Tbcreated,
                Tbmodified = request.Tbmodified,
                TbcreatedId = request.TbcreatedId,
                TbmodifiedId = request.TbmodifiedId,
                Tbguid = request.Tbguid
            };
        }

        public static List<OmmessageInfoRequestDto> ToOmmessageInforequestDto(this List<VwOmmessage> request)
        {
            var ommessageInfoRequestDto = request
            .GroupBy(x => x.Moid)
            .Select(g => new OmmessageInfoRequestDto
            {
                Moid = g.Key,
                OmmessageDetails = g.Select(x => new OmmessageDetailsRequestDto
                {
                    MessageId = x.MessageId,
                    MessageType = x.MessageType,
                    WorkerId = x.WorkerId,
                    MessageDate = x.MessageDate,
                    MessageText = x.MessageText,
                    UserMessage = x.UserMessage,
                    Expire = x.Expire,
                    ExpirationDate = x.ExpirationDate,
                    RtgStep = x.RtgStep,
                    Alternate = x.Alternate,
                    AltRtgStep = x.AltRtgStep,
                    Tbcreated = x.Tbcreated,
                    Tbmodified = x.Tbmodified,
                    TbcreatedId = x.TbcreatedId,
                    TbmodifiedId = x.TbmodifiedId,
                    Tbguid = x.Tbguid
                }).ToList()
            }).ToList();
            return ommessageInfoRequestDto;
        }
    }
}
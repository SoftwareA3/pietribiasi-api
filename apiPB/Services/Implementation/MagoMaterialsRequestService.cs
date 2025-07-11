using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;
using apiPB.ApiClient.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Repository.Abstraction;
using AutoMapper;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Services.Abstraction;
using apiPB.Dto.Request;
using Azure.Core;
using apiPB.Utils.Abstraction;

namespace apiPB.Services.Implementation
{
    public class MagoMaterialsRequestService : IMagoMaterialsRequestService
    {
        private readonly IMagoApiClient _magoApiClient;
        //private readonly IPrelMatRequestService _prelMatRequestService;
        //private readonly ISettingsRepository _settingsRepository;
        private readonly ILogService _logService;

        public MagoMaterialsRequestService(
            IMagoApiClient magoApiClient,
            //IPrelMatRequestService prelMatRequestService,
            //ISettingsRepository settingsRepository,
            ILogService logService)
        {
            _magoApiClient = magoApiClient;
            //_prelMatRequestService = prelMatRequestService;
            //_settingsRepository = settingsRepository;
            _logService = logService;
        }

        public async Task<IEnumerable<DeleteMoComponentRequestDto>> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, IEnumerable<DeleteMoComponentRequestDto> request)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }

            var response = await _magoApiClient.SendPostAsyncWithToken<DeleteMoComponentRequestDto>(
                "PrelMat/DeleteMoComponent",
                request,
                responseDto.Token, false);
                
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncRegOre failed");
            }
            Console.WriteLine($"SyncRegOre successfull response: {response}");

            return request;
        }
    }
}
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
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogService _logService;

        public MagoMaterialsRequestService(
            IMagoApiClient magoApiClient,
            //IPrelMatRequestService prelMatRequestService,
            ISettingsRepository settingsRepository,
            ILogService logService)
        {
            _magoApiClient = magoApiClient;
            //_prelMatRequestService = prelMatRequestService;
            _settingsRepository = settingsRepository;
            _logService = logService;
        }

        public async Task<DeleteMoComponentRequestDto> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, DeleteMoComponentRequestDto request)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }

            var requestList = new List<DeleteMoComponentRequestDto> { request };
            var response = await _magoApiClient.SendPostAsyncWithToken<DeleteMoComponentRequestDto>(
                "openMes/delete-mo-component",
                requestList,
                responseDto.Token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncRegOre failed");
            }
            Console.WriteLine($"SyncRegOre successfull response: {response}");

            return request;
        }

        public async Task<AddMoComponentRequestDto> AddMoComponentAsync(MagoLoginResponseDto responseDto, AddMoComponentRequestDto request)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }

            var settings = _settingsRepository.GetSettings() ?? throw new InvalidOperationException("Settings cannot be null");

            // Chiamare Mapper per la conversione al formato per l'API di Mago apposito (per inserire i valori di default dove servono)
            var mappedRequest = settings.ToAddMaterialSyncPrelMatRequestDto(request);

            try
            {
                // Chiamare API di Mago per l'inserimento del materiale
                var response = await _magoApiClient.SendPostAsyncWithToken<SyncPrelMatRequestDto>(
                    "openMes/materials-picking",
                    mappedRequest,
                    responseDto.Token);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception("AddMoComponent failed");
                }
                Console.WriteLine($"AddMoComponent successfull response: {response}");

                // Incrementare il contatore per il riferimento esterno
                _settingsRepository.IncrementExternalReferenceCounter();

                return request;
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in AddMoComponent: {ex.Message}");
                Console.WriteLine("Error sending data to Mago: logging off..." + ex.Message);

                throw;
            }
        }
    }
}
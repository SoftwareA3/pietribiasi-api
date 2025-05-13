using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;
using apiPB.ApiClient.Abstraction;

namespace apiPB.Services.Implementation
{
    public class MagoAccessService : IMagoAccessService
    {
        private readonly IMagoApiClient _magoApiClient;


        public MagoAccessService(IMagoApiClient magoApiClient)
        {
            _magoApiClient = magoApiClient;
        }

        public async Task<MagoLoginResponseDto> LoginAsync(MagoLoginRequestDto request)
        {
            var response = await _magoApiClient.SendPostAsync("account-manager/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MagoLoginResponseDto>();

                return result ?? new MagoLoginResponseDto();
            }
            else
            {
                throw new Exception("Login failed");
            }
        }

        public async Task LogoffAsync(MagoTokenRequestDto dto)
        {
            var response = await _magoApiClient.SendPostAsyncWithToken("account-manager/logout", dto, dto.Token ?? string.Empty);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Errore nel logout. Stato: {response.StatusCode}");
            }
        }
    }
}
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

namespace apiPB.Services.Implementation
{
    public class MagoRequestService : IMagoRequestService
    {
        // ApiClient
        private readonly IMagoApiClient _magoApiClient;

        private readonly IRegOreRequestService _regoreRequestService;
        private readonly IPrelMatRequestService _prelMatRequestService;
        private readonly IInventarioRequestService _inventarioRequestService;
        private readonly IMagoRepository _magoRepository;

        // Mapper
        private readonly IMapper _mapper;

        public MagoRequestService(IMagoApiClient magoApiClient, 
            IMapper mapper,
            IMagoRepository magoRepository,
            IRegOreRequestService regOreRequestService,
            IPrelMatRequestService prelMatRequestService,
            IInventarioRequestService inventarioRequestService)
        {
            _magoApiClient = magoApiClient;
            _magoRepository = magoRepository;
            _mapper = mapper;
            _regoreRequestService = regOreRequestService;
            _prelMatRequestService = prelMatRequestService;
            _inventarioRequestService = inventarioRequestService;
        }

        public async Task SyncronizeAsync(WorkerIdSyncRequestDto request)
        {
            if(request == null || request.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Recupero delle impostazioni da Mago. Attacca il WorkerId
            var settings = _magoRepository.GetSettings();
            if (settings == null)
            {
                throw new Exception("Settings not found");
            }

            // Crea il dto per il login
            var magoLoginRequest = settings.ToMagoLoginRequestDto();

            // Login
            var loginResponse = await LoginAsync(magoLoginRequest);
            if (loginResponse == null || loginResponse.Token == null)
            {
                throw new Exception("Login failed");
            }

            var token = loginResponse.Token;
            Console.WriteLine($"Token: {token}");

            // Crea il Dto con tutti i dati per effettuare le richieste all'API di Mago
            var magoApiRequest = settings.ToSyncSettingsRequestDto(request.WorkerId);

            // Recupero Lista di record da A3_app_reg_ore
            var regOreList = _regoreRequestService.GetNotImportedAppRegOre();

            if (regOreList == null)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("No records found in A3_app_reg_ore: logging off...");
            }

            // Mapping delle informazioni per l'invio a Mago. Dati di settings + A3_app_reg_ore
            var syncRegOreList = magoApiRequest.ToSyncregOreRequestDto(regOreList.ToList());

            // Invio Lista di record a Mago assieme a magoApiRequest
            try
            {
                var syncRegOreResponse = SyncRegOre(syncRegOreList, token);
            }
            catch (Exception ex)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("Error sending data to Mago: logging off...", ex);
            }

            // Aggiornamento della lista di record in A3_app_reg_ore 
            var regOreListUpdated = _regoreRequestService.UpdateRegOreImported(request);  
            if(regOreListUpdated == null)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("No records updated in A3_app_reg_ore: logging off...");
            }

            // Recupero Lista di record da A3_app_prel_mat
            var prelMatList = _prelMatRequestService.GetNotImportedPrelMat();
            if (prelMatList == null)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                // Da correggere con NoContent
                throw new Exception("No records found in A3_app_prel_mat");
            }

            // Mapping delle informazioni per l'invio a Mago. Dati di settings + A3_app_prel_mat
            var syncPrelMatList = magoApiRequest.ToSyncPrelMatRequestDto(prelMatList.ToList());
            
            // Invio Lista di record a Mago
            try
            {
                var syncPrelMatResponse = SyncPrelMat(syncPrelMatList, token);
            }
            catch (Exception ex)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("Error sending data to Mago: logging off...", ex);
            }

            // Aggiornamento della lista di record in A3_app_prel_mat
            var prelMatListUpdated = _prelMatRequestService.UpdatePrelMatImported(request);
            if (prelMatListUpdated == null)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("No records updated in A3_app_prel_mat: logging off...");
            } 

            // Recupero Lista di record da A3_app_inventario
            
            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_inventario

            // Se le operazioni vanno a buon fine, invio Logout

            try
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Logoff failed", ex);
            }
        }

        public async Task SyncRegOre(IEnumerable<SyncRegOreRequestDto> request, string token)
        {
            if (request == null || token == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await _magoApiClient.SendPostAsyncWithToken("openMes/mo-confirmation", request, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncRegOre failed");
            }
            Console.WriteLine("Logoff successful");
        }

        public async Task SyncPrelMat(IEnumerable<SyncPrelMatRequestDto> request, string token)
        {
            if (request == null || token == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await _magoApiClient.SendPostAsyncWithToken("openMes/mo-confirmation", request, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncPrelMat failed");
            }
        }

        public async Task<MagoLoginResponseDto?> LoginAsync(MagoLoginRequestDto request)
        {
            var response = await _magoApiClient.SendPostAsync("account-manager/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MagoLoginResponseDto>();
                if (result == null)
                {
                    return null;
                }

                return result;
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

        public SettingsDto EditSettings(SettingsDto settings)
        {
            var filter = _mapper.Map<SettingsFilter>(settings);
            _magoRepository.EditSettings(filter);

            return settings;
        }

        public SettingsDto? GetSettings()
        {
            var settings = _magoRepository.GetSettings();
            if (settings != null)
            {
                return settings;
            }
            else
            {
                return null;
            }
        }
    }
}
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

        // Repository
        private readonly IRegOreRepository _regOreRepository;
        private readonly IPrelMatRepository _prelMatRepository;
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IMagoRepository _magoRepository;

        // Service
        private readonly IMagoAccessService _magoAccessService;

        // Mapper
        private readonly IMapper _mapper;

        public MagoRequestService(IMagoApiClient magoApiClient, 
            IMagoAccessService magoAccessService,
            IMapper mapper,
            IMagoRepository magoRepository,
            IRegOreRepository regOreRepository,
            IPrelMatRepository prelMatRepository,
            IInventarioRepository inventarioRepository)
        {
            _magoApiClient = magoApiClient;
            _magoRepository = magoRepository;
            _mapper = mapper;
            _magoAccessService = magoAccessService;
            _regOreRepository = regOreRepository;
            _prelMatRepository = prelMatRepository;
            _inventarioRepository = inventarioRepository;
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
            var loginResponse = await _magoAccessService.LoginAsync(magoLoginRequest);
            if (loginResponse == null || loginResponse.Token == null)
            {
                throw new Exception("Login failed");
            }

            var token = loginResponse.Token;
            Console.WriteLine($"Token: {token}");

            // Crea il Dto con tutti i dati per effettuare le richieste all'API di Mago
            var magoApiRequest = settings.ToSyncSettingsRequestDto(request.WorkerId);

            // Recupero Lista di record da A3_app_reg_ore
            var regOreList = _regOreRepository.GetAppRegOre()
                .Select(x => x.ToA3AppRegOreDto());

            if (regOreList == null || !regOreList.Any())
            {
                await _magoAccessService.LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("No records found in A3_app_reg_ore: logging off...");
            }

            List<SyncRegOreRequestDto> syncRegOreList = new List<SyncRegOreRequestDto>();

            // Mapping delle informazioni per l'invio a Mago. Dati di settings + A3_app_reg_ore
            foreach (var regOre in regOreList)
            {
                syncRegOreList.Add(magoApiRequest.ToSyncregOreRequestDto(regOre));
            }

            // Invio Lista di record a Mago assieme a magoApiRequest
            try
            {
                var syncRegOreResponse = SyncRegOre(syncRegOreList, token);
            }
            catch (Exception ex)
            {
                await _magoAccessService.LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("Error sending data to Mago: logging off...", ex);
            }

            // Aggiornamento della lista di record in A3_app_reg_ore 
            var regOreListUpdated = _regOreRepository.UpdateRegOreImported(request.WorkerId);  
            if(regOreListUpdated != null && regOreListUpdated.Any())
            {
                regOreListUpdated.Select(x => x.ToA3AppRegOreDto());
            }
            else
            {
                await _magoAccessService.LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                throw new Exception("No records updated in A3_app_reg_ore: logging off...");
            }

            // Recupero Lista di record da A3_app_prel_mat
            // var prelMatList = _prelMatRepository.GetAppPrelMat();
            // if (prelMatList == null || !prelMatList.Any())
            // {
            //     throw new Exception("No records found in A3_app_prel_mat");
            // }

            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_prel_mat
            //var prelMatListUpdated = _prelMatRepository.UpdatePrelMatImported(request.WorkerId);

            // Recupero Lista di record da A3_app_inventario
            
            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_inventario

            // Se le operazioni vanno a buon fine, invio Logout

            try
            {
                await _magoAccessService.LogoffAsync(new MagoTokenRequestDto
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
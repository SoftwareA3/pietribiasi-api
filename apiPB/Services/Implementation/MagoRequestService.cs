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
    public class MagoRequestService : IMagoRequestService
    {
        // ApiClient
        private readonly IMagoApiClient _magoApiClient;

        private readonly IRegOreRequestService _regoreRequestService;
        private readonly IPrelMatRequestService _prelMatRequestService;
        private readonly IInventarioRequestService _inventarioRequestService;
        private readonly IMagoRepository _magoRepository;

        private readonly ILogService _logService;

        // Mapper
        private readonly IMapper _mapper;

        public MagoRequestService(IMagoApiClient magoApiClient,
            IMapper mapper,
            IMagoRepository magoRepository,
            IRegOreRequestService regOreRequestService,
            IPrelMatRequestService prelMatRequestService,
            IInventarioRequestService inventarioRequestService,
            ILogService logService)
        {
            _magoApiClient = magoApiClient;
            _magoRepository = magoRepository;
            _mapper = mapper;
            _regoreRequestService = regOreRequestService;
            _prelMatRequestService = prelMatRequestService;
            _inventarioRequestService = inventarioRequestService;
            _logService = logService;
        }

        public async Task<SyncronizedDataDto> SyncronizeAsync(WorkerIdSyncRequestDto request)
        {
            // Recupero delle impostazioni da Mago. Attacca il WorkerId
            var settings = _magoRepository.GetSettings();
            if (settings == null)
            {
                throw new Exception("Settings not found");
            }

            // Crea il dto da ritornare
            var syncData = new SyncronizedDataDto();

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
            var magoApiRequest = settings;

// ==========================================================================================================

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

            if (syncRegOreList != null && syncRegOreList.Count > 0)
            {
                // Invio Lista di record a Mago assieme a magoApiRequest
                try
                {
                    Console.WriteLine($"SyncRegOreList: {syncRegOreList}");

                    var response = await SyncRegOre(syncRegOreList, token);

                    // Aggiornamento della lista di record in A3_app_reg_ore 
                    // A prescindere dal risultato della chiamata a Mago, aggiorna i record in A3_app_reg_ore
                    Console.WriteLine("Updating records in A3_app_reg_ore...");
                    var regOreListUpdated = _regoreRequestService.UpdateRegOreImported(request);
                    Console.WriteLine($"RegOreListUpdated: {regOreListUpdated}");
                    if (regOreListUpdated == null)
                    {
                        await LogoffAsync(new MagoTokenRequestDto
                        {
                            Token = token
                        });
                        throw new Exception("No records updated in A3_app_reg_ore: logging off...");
                    }

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncRegOre failed");
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncRegOre: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }
            }
            else
            {
                Console.WriteLine("SyncRegOreList is null or empty: no data to send and therefore ignored");
            }

// =========================================================================================================

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
            Console.WriteLine($"PrelMatList: {prelMatList}");

            // Mapping delle informazioni per l'invio a Mago. Dati di settings + A3_app_prel_mat
            var syncPrelMatList = magoApiRequest.ToSyncPrelMatRequestDto(prelMatList.ToList());

            if (syncPrelMatList != null && syncPrelMatList.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"SyncPrelMatList: {syncPrelMatList}");
                    var response = await SyncPrelMat(syncPrelMatList, token);
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncPrelMat failed");
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncPrelMat: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }

                // Aggiornamento della lista di record in A3_app_prel_mat
                Console.WriteLine("Updating records in A3_app_prel_mat...");
                var prelMatListUpdated = _prelMatRequestService.UpdatePrelMatImported(request);
                Console.WriteLine($"PrelMatListUpdated: {prelMatListUpdated}");
                if (prelMatListUpdated == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("No records updated in A3_app_prel_mat: logging off...");
                }
            }
            else
            {
                Console.WriteLine("SyncPrelMatList is null or empty: no data to send and therefore ignored");
            }

// ==========================================================================================================

            // Recupero Lista di record da A3_app_inventario
            var inventarioList = _inventarioRequestService.GetNotImportedInventario();
            if (inventarioList == null)
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                // Da correggere con NoContent
                throw new Exception("No records found in A3_app_inventario");
            }
            Console.WriteLine($"InventarioList: {inventarioList}");

            // Mapping delle informazioni per l'invio a Mago. Dati di settings + A3_app_inventario
            var syncInventarioList = magoApiRequest.ToSyncInventarioRequestDto(inventarioList.ToList());
            if (syncInventarioList != null && syncInventarioList.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"SyncInventarioList: {syncInventarioList}");
                    foreach (var item in syncInventarioList)
                    {
                        List<SyncInventarioRequestDto> itemList = new List<SyncInventarioRequestDto> { item };
                        var response = await SyncInventario(itemList, token);
                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new Exception("SyncInventario failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncInventario: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }

                // Aggiornamento della lista di record in A3_app_inventario
                Console.WriteLine("Updating records in A3_app_inventario...");
                var inventarioListUpdated = _inventarioRequestService.UpdateInventarioImported(request);
                Console.WriteLine($"InventarioListUpdated: {inventarioListUpdated}");
                if (inventarioListUpdated == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("No records updated in A3_app_inventario: logging off...");
                }
            }
            else
            {
                Console.WriteLine("SyncInventarioList is null or empty: no data to send and therefore ignored");
            }

// ==========================================================================================================

            // Se le operazioni vanno a buon fine, invio Logout
            try
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                Console.WriteLine($"Data sent to Mago and logged off successfully. Data: {syncData}");
                return syncData.ToSyncronizedDataDto(syncRegOreList, syncPrelMatList, syncInventarioList);
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in Logoff: {ex.Message}");
                throw new Exception("Logoff failed", ex);
            }
        }

        public async Task<HttpResponseMessage> SyncRegOre(IEnumerable<SyncRegOreRequestDto> request, string token)
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
            Console.WriteLine($"SyncRegOre successfull response: {response}");

            return response;
        }

        public async Task<HttpResponseMessage> SyncPrelMat(IEnumerable<SyncPrelMatRequestDto> request, string token)
        {
            if (request == null || token == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await _magoApiClient.SendPostAsyncWithToken("openMes/materials-picking", request, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncPrelMat failed");
            }
            Console.WriteLine($"SyncPrelMat successfull response: {response}");

            return response;
        }

        public async Task<HttpResponseMessage> SyncInventario(IEnumerable<SyncInventarioRequestDto> request, string token)
        {
            if (request == null || token == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await _magoApiClient.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries", request, token, false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncInventario failed");
            }
            Console.WriteLine($"SyncInventario successfull response: {response}");
            return response;
        }

        public async Task<SyncRegOreRequestDto> SyncRegOreFiltered(ViewOreRequestDto? request)
        {
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
            var magoApiRequest = settings;
            var syncRegOreRequest = new List<RegOreDto>();

            if (request == null)
            {
                syncRegOreRequest = _regoreRequestService.GetNotImportedAppRegOre().ToList();
                if (syncRegOreRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_inventario");
                }
            }
            else
            {
                syncRegOreRequest = _regoreRequestService.GetNotImportedAppRegOreByFilter(request).ToList();
                if (syncRegOreRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_inventario");
                }
            }

            var regOreList = magoApiRequest.ToSyncregOreRequestDto(syncRegOreRequest);
            if (regOreList != null && regOreList.Count > 0)
            {
                // Invio Lista di record a Mago assieme a magoApiRequest
                try
                {
                    Console.WriteLine($"SyncRegOreList: {regOreList}");

                    var response = await SyncRegOre(regOreList, token);

                    // Aggiornamento della lista di record in A3_app_reg_ore 
                    // A prescindere dal risultato della chiamata a Mago, aggiorna i record in A3_app_reg_ore
                    Console.WriteLine("Updating records in A3_app_reg_ore...");
                    var regOreListUpdated = new List<RegOreDto>();

                    foreach (var item in syncRegOreRequest)
                    {
                        regOreListUpdated.AddRange(_regoreRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                        {
                            Id = item.RegOreId,
                            WorkerId = request.WorkerId
                        }));
                    }
                    
                    Console.WriteLine($"RegOreListUpdated: {regOreListUpdated}");
                    if (regOreListUpdated == null)
                    {
                        await LogoffAsync(new MagoTokenRequestDto
                        {
                            Token = token
                        });
                        throw new Exception("No records updated in A3_app_reg_ore: logging off...");
                    }

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncRegOre failed");
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncRegOre: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }
            }
            else
            {
                Console.WriteLine("SyncRegOreList is null or empty: no data to send and therefore ignored");
            }

            try
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                return regOreList.FirstOrDefault() ?? new SyncRegOreRequestDto();
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in Logoff: {ex.Message}");
                throw new Exception("Logoff failed", ex);
            }
        }

        public async Task<SyncPrelMatRequestDto> SyncPrelMatFiltered(ViewPrelMatRequestDto request)
        {
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
            var magoApiRequest = settings;
            var syncPrelMatRequest = new List<PrelMatDto>();

            if (request == null)
            {
                syncPrelMatRequest = _prelMatRequestService.GetNotImportedPrelMat().ToList();
                if (syncPrelMatRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_prel_mat");
                }
            }
            else
            {
                syncPrelMatRequest = _prelMatRequestService.GetNotImportedAppPrelMatByFilter(request).ToList();
                if (syncPrelMatRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_prel_mat");
                }
            }
            var prelMatList = magoApiRequest.ToSyncPrelMatRequestDto(syncPrelMatRequest);

            if (prelMatList != null && prelMatList.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"SyncPrelMatList: {prelMatList}");
                    var response = await SyncPrelMat(prelMatList, token);
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncPrelMat failed");
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncPrelMat: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }

                // Aggiornamento della lista di record in A3_app_prel_mat
                Console.WriteLine("Updating records in A3_app_prel_mat...");
                var prelMatListUpdated = new List<PrelMatDto>();

                foreach (var item in syncPrelMatRequest)
                {
                    prelMatListUpdated.AddRange(_prelMatRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                    {
                        Id = item.PrelMatId,
                        WorkerId = request.WorkerId
                    }));
                }
                
                Console.WriteLine($"PrelMatListUpdated: {prelMatListUpdated}");
                if (prelMatListUpdated == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("No records updated in A3_app_prel_mat: logging off...");
                }
            }
            else
            {
                Console.WriteLine("SyncPrelMatList is null or empty: no data to send and therefore ignored");
            }
            try
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                return prelMatList.FirstOrDefault() ?? new SyncPrelMatRequestDto();
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in Logoff: {ex.Message}");
                throw new Exception("Logoff failed", ex);
            }
        }

        public async Task<SyncInventarioRequestDto> SyncInventarioFiltered(ViewInventarioRequestDto request)
        {
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
            var magoApiRequest = settings;
            var syncInventarioRequest = new List<InventarioDto>();

            if (request == null)
            {
                syncInventarioRequest = _inventarioRequestService.GetNotImportedInventario().ToList();
                if (syncInventarioRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_inventario");
                }
            }
            else
            {
                syncInventarioRequest = _inventarioRequestService.GetNotImportedAppInventarioByFilter(request).ToList();
                if (syncInventarioRequest == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    // Da correggere con NoContent
                    throw new Exception("No records found in A3_app_inventario");
                }
            }

            var inventarioList = magoApiRequest.ToSyncInventarioRequestDto(syncInventarioRequest);

            if (inventarioList != null && inventarioList.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"SyncInventarioList: {inventarioList}");
                    foreach (var item in inventarioList)
                    {
                        List<SyncInventarioRequestDto> itemList = new List<SyncInventarioRequestDto> { item };
                        var response = await SyncInventario(itemList, token);
                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new Exception("SyncInventario failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncInventario: {ex.Message}");
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("Error sending data to Mago: logging off...", ex);
                }
                // Aggiornamento della lista di record in A3_app_inventario
                Console.WriteLine("Updating records in A3_app_inventario...");
                var inventarioListUpdated = new List<InventarioDto>();
                foreach (var item in syncInventarioRequest)
                {
                    inventarioListUpdated.AddRange(_inventarioRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                    {
                        Id = item.InvId,
                        WorkerId = request.WorkerId
                    }));
                }
                Console.WriteLine($"InventarioListUpdated: {inventarioListUpdated}");
                if (inventarioListUpdated == null)
                {
                    await LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = token
                    });
                    throw new Exception("No records updated in A3_app_inventario: logging off...");
                }
            }
            else
            {
                Console.WriteLine("SyncInventarioList is null or empty: no data to send and therefore ignored");
            }
            try
            {
                await LogoffAsync(new MagoTokenRequestDto
                {
                    Token = token
                });
                return inventarioList.FirstOrDefault() ?? new SyncInventarioRequestDto();
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in Logoff: {ex.Message}");
                throw new Exception("Logoff failed", ex);
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
            var listLogoff = new List<MagoTokenRequestDto> { dto };
            var response = await _magoApiClient.SendPostAsyncWithToken<MagoTokenRequestDto>("account-manager/logout", listLogoff, dto.Token ?? string.Empty);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Errore nel logout. Stato: {response.StatusCode}");
            }
        }

        public SettingsDto EditSettings(SettingsDto settings)
        {
            var filter = _mapper.Map<SettingsFilter>(settings);
            var editedSettings = _magoRepository.EditSettings(filter);

            return editedSettings.ToSettingsDto();
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

        public SyncGobalActiveRequestDto? GetSyncGlobalActive()
        {
            var appSetting = _magoRepository.GetSyncGlobalActive();
            if (appSetting != null && appSetting.SyncGlobalActive != null)
            {
                return appSetting;
            }
            else
            {
                return null;
            }
        }
    }
}
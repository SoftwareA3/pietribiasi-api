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
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogService _logService;
        // Mapper
        private readonly IMapper _mapper;

        public MagoRequestService(IMagoApiClient magoApiClient,
            IMapper mapper,
            ISettingsRepository settingsRepository,
            IRegOreRequestService regOreRequestService,
            IPrelMatRequestService prelMatRequestService,
            IInventarioRequestService inventarioRequestService,
            ILogService logService)
        {
            _magoApiClient = magoApiClient;
            _settingsRepository = settingsRepository;
            _mapper = mapper;
            _regoreRequestService = regOreRequestService;
            _prelMatRequestService = prelMatRequestService;
            _inventarioRequestService = inventarioRequestService;
            _logService = logService;
        }

        public async Task<SyncronizedDataDto> SyncronizeAsync(MagoLoginResponseDto responseDto, SettingsDto settings, WorkerIdSyncRequestDto requestId)
        {
            try
            {
                var syncDataListToReturn = new SyncronizedDataDto();

                var syncRegOreList = await SyncRegOreFiltered(responseDto, settings, new SyncRegOreFilteredDto { WorkerIdSyncRequestDto = requestId }, false);

                var syncPrelMatList = await SyncPrelMatFiltered(responseDto, settings, new SyncPrelMatFilteredDto { WorkerIdSyncRequestDto = requestId }, false);

                var syncInventarioList = await SyncInventarioFiltered(responseDto, settings, new SyncInventarioFilteredDto { WorkerIdSyncRequestDto = requestId }, false);

                // ======================================================
                // Se le operazioni vanno a buon fine, invia Logout
                // ======================================================

                // Riassegnazione della lista creata inizialmente (FIXME)
                syncDataListToReturn = syncDataListToReturn.ToSyncronizedDataDto(syncRegOreList.ToList(), syncPrelMatList.ToList(), syncInventarioList.ToList());
                Console.WriteLine($"Data sent to Mago and logged off successfully. Data: {syncDataListToReturn}");
                return syncDataListToReturn;
            }
            catch (Exception ex)
            {
                _logService.AppendErrorToLog($"Error in SynchronizeAsync: {ex.Message}");
                Console.WriteLine("Global Synchronization failed" + ex.Message);

                throw;
            }
        }

        public async Task<IEnumerable<SyncRegOreRequestDto>> SyncRegOreFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncRegOreFilteredDto? request, bool isFiltered = true)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || settings == null || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }
            List<RegOreDto> syncRegOreRequest;

            if (request.RegOreList != null && request.RegOreList.Count > 0 && isFiltered)
            {
                Console.WriteLine($"Filter applied ViewRegOre: {request.RegOreList}");
                syncRegOreRequest = request.RegOreList.ToList();

                if (syncRegOreRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_reg_ore");
                    throw new Exception("No records found in A3_app_reg_ore");
                }
            }
            else
            {
                Console.WriteLine("No filter applied, retrieving all records from A3_app_reg_ore");
                syncRegOreRequest = _regoreRequestService.GetNotImportedAppRegOre().ToList();

                if (syncRegOreRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_reg_ore");
                    throw new Exception("No records found in A3_app_reg_ore");
                }
            }

            // Crea il Dto con tutti i dati per effettuare le richieste all'API di Mago
            var regOreList = settings.ToSyncregOreRequestDto(syncRegOreRequest);

            if (regOreList != null && regOreList.Count > 0)
            {
                // Invio Lista di record a Mago assieme a magoApiRequest
                try
                {
                    Console.WriteLine($"SyncRegOreList: {regOreList}");

                    var response = await SyncRegOre(regOreList, responseDto.Token);
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncRegOre failed");
                    }

                    // Aggiornamento della lista di record in A3_app_reg_ore 
                    Console.WriteLine("Updating records in A3_app_reg_ore...");
                    // Se la sincronizzaizone è generale o se è generale e non sono state passate liste filtrate
                    // Aggiorna tutti i record. Quest'operaizone è fatta per evitare foreach inutili.
                    if (!isFiltered || (!isFiltered && request.RegOreList?.Count == 0))
                    {
                        var regOreListUpdatedByWorkerId = _regoreRequestService.UpdateRegOreImported(request.WorkerIdSyncRequestDto);
                        Console.WriteLine($"RegOreListUpdatedByWorkerId: {regOreListUpdatedByWorkerId}");
                        if (regOreListUpdatedByWorkerId == null)
                        {
                            throw new Exception("No records updated in A3_app_reg_ore by WorkerId: logging off...");
                        }
                    }
                    // In questo caso la lista è una lista filtrata e la sincronizzaiozne non è generale
                    // Quindi aggiorna i record in base al WorkerId passato e all'Id dei record da aggiornare
                    else
                    {
                        foreach (var item in syncRegOreRequest)
                        {
                            _regoreRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                            {
                                Id = item.RegOreId,
                                WorkerId = request.WorkerIdSyncRequestDto.WorkerId
                            });
                        }
                    }

                    return regOreList;
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncRegOre: {ex.Message}");
                    Console.WriteLine("Error sending data to Mago: logging off..." + ex.Message);

                    throw;
                }
            }
            else
            {
                Console.WriteLine("SyncRegOreList is null or empty: no data to send and therefore ignored");
                return new List<SyncRegOreRequestDto>();
            }
        }

        public async Task<IEnumerable<SyncPrelMatRequestDto>> SyncPrelMatFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncPrelMatFilteredDto? request, bool isFiltered = true)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || settings == null || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }
            List<PrelMatDto> syncPrelMatRequest;

            if (request.PrelMatList != null && request.PrelMatList.Count > 0 && isFiltered)
            {
                Console.WriteLine($"Filter applied ViewPrelMat: {request.PrelMatList}");
                syncPrelMatRequest = request.PrelMatList.ToList();

                if (syncPrelMatRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_prel_mat");
                    throw new Exception("No records found in A3_app_prel_mat");
                }
            }
            else
            {
                Console.WriteLine("No filter applied, retrieving all records from A3_app_prel_mat");
                // QUA LI PRENDE SENZA FILTRO PER I MATERIALI DA ELIMINARE
                syncPrelMatRequest = _prelMatRequestService.GetNotImportedPrelMat().ToList();

                if (syncPrelMatRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_prel_mat");
                    throw new Exception("No records found in A3_app_prel_mat");
                }
            }

            // Estrae i materiali da non eliminare
            var syncPrelMatListNotDeleted = syncPrelMatRequest
                .Where(p => p.Deleted == false)
                .ToList();

            var prelMatList = settings.ToSyncPrelMatRequestDto(syncPrelMatListNotDeleted); 

            if (syncPrelMatRequest != null && syncPrelMatRequest.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"SyncPrelMatList: {prelMatList}");
                    var response = await SyncPrelMat(prelMatList, responseDto.Token);
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception("SyncPrelMat failed");
                    }

                    // Aggiornamento della lista di record in A3_app_prel_mat
                    Console.WriteLine("Updating records in A3_app_prel_mat...");
                    // Se la sincronizzaizone è generale o se è generale e non sono state passate liste filtrate
                    // Aggiorna tutti i record. Quest'operaizone è fatta per evitare foreach inutili.
                    if (!isFiltered || (!isFiltered && syncPrelMatListNotDeleted.Count == 0))
                    {
                        var prelMatListUpdated = _prelMatRequestService.UpdatePrelMatImported(request.WorkerIdSyncRequestDto);
                        Console.WriteLine($"PrelMatListUpdated: {prelMatListUpdated}");
                        if (prelMatListUpdated == null)
                        {
                            throw new Exception("No records updated in A3_app_prel_mat: logging off...");
                        }
                    }
                    // In questo caso la lista è una lista filtrata e la sincronizzaiozne non è generale
                    // Quindi aggiorna i record in base al WorkerId passato e all'Id dei record da aggiornare
                    else
                    {
                        foreach (var item in syncPrelMatListNotDeleted)
                        {
                            _prelMatRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                            {
                                Id = item.PrelMatId,
                                WorkerId = request.WorkerIdSyncRequestDto.WorkerId
                            });
                        }
                    }

                    _settingsRepository.IncrementExternalReferenceCounter();

                    Console.WriteLine($"ExternalReferenceCounter incremented: {settings.ExternalReferences}");

                    // Sincronizzazione materiali da eliminare
                    var deletedPrelMatList = syncPrelMatRequest
                        .Where(p => p.Deleted == true)
                        .ToList();

                    if (deletedPrelMatList.Count > 0)
                    {
                        Console.WriteLine($"SyncPrelMatList Deleted: {deletedPrelMatList}");

                        var responseDelete = await DeleteMoComponentAsync(responseDto, deletedPrelMatList);
                        if (responseDelete.StatusCode != HttpStatusCode.OK && responseDelete.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new Exception("SyncPrelMat failed");
                        }

                        Console.WriteLine($"SyncPrelMatList Deleted response: {responseDelete}");

                        Console.WriteLine("Updating deleted records in A3_app_prel_mat...");
                        // Aggiorna i record eliminati in A3_app_prel_mat
                        if (!isFiltered)
                        {
                            var deletedPrelMatListUpdated = _prelMatRequestService.UpdatePrelMatImported(request.WorkerIdSyncRequestDto, true);
                            Console.WriteLine($"DeletedPrelMatListUpdated: {deletedPrelMatListUpdated}");
                            if (deletedPrelMatListUpdated == null)
                            {
                                throw new Exception("No deleted records updated in A3_app_prel_mat: logging off...");
                            }
                        }
                        else
                        {
                            foreach (var item in deletedPrelMatList)
                            {
                                _prelMatRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                                {
                                    Id = item.PrelMatId,
                                    WorkerId = request.WorkerIdSyncRequestDto.WorkerId
                                }, true);
                            }
                        }

                        _settingsRepository.IncrementExternalReferenceCounter();

                        prelMatList.AddRange(settings.ToSyncPrelMatRequestDto(deletedPrelMatList));
                    }


                    return prelMatList;
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncPrelMat: {ex.Message}");
                    Console.WriteLine("Error sending data to Mago: logging off..." + ex.Message);

                    throw;
                }
            }
            else
            {
                Console.WriteLine("SyncPrelMatList is null or empty: no data to send and therefore ignored");
                return new List<SyncPrelMatRequestDto>();
            }
        }

        public async Task<IEnumerable<SyncInventarioRequestDto>> SyncInventarioFiltered(MagoLoginResponseDto responseDto, SettingsDto settings, SyncInventarioFilteredDto? request, bool isFiltered = true)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || settings == null || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }
            List<InventarioDto> syncInventarioRequest;

            if (request.InventarioList != null && request.InventarioList.Count > 0 && isFiltered)
            {
                syncInventarioRequest = request.InventarioList.ToList();

                if (syncInventarioRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_inventario");
                    throw new Exception("No records found in A3_app_inventario");
                }
                Console.WriteLine($"SyncInventarioRequest: {syncInventarioRequest.Count}");
            }
            else
            {
                Console.WriteLine("No filter applied, retrieving all records from A3_app_inventario");
                syncInventarioRequest = _inventarioRequestService.GetNotImportedInventario().ToList();

                if (syncInventarioRequest == null)
                {
                    _logService.AppendErrorToLog("No records found in A3_app_inventario");
                    throw new Exception("No records found in A3_app_inventario");
                }
            }

            // Crea il Dto con tutti i dati per effettuare le richieste all'API di Mago
            var inventarioList = settings.ToSyncInventarioRequestDto(syncInventarioRequest);

            if (inventarioList != null && inventarioList.Count > 0)
            {
                // Invio Lista di record a Mago
                try
                {
                    Console.WriteLine($"InventarioList pre Sync Attempt: {inventarioList}");
                    foreach (var item in inventarioList)
                    {
                        List<SyncInventarioRequestDto> itemList = new List<SyncInventarioRequestDto> { item };
                        var response = await SyncInventario(itemList, responseDto.Token);
                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new Exception("SyncInventario failed");
                        }
                    }

                    // Aggiornamento della lista di record in A3_app_inventario
                    Console.WriteLine("Updating records in A3_app_inventario...");
                    // Se la sincronizzaizone è generale o se è generale e non sono state passate liste filtrate
                    // Aggiorna tutti i record. Quest'operaizone è fatta per evitare foreach inutili.
                    if (!isFiltered || (!isFiltered && request.InventarioList?.Count == 0))
                    {
                        var inventarioListUpdated = _inventarioRequestService.UpdateInventarioImported(request.WorkerIdSyncRequestDto);
                        if (inventarioListUpdated == null)
                        {
                            throw new Exception("No records updated in A3_app_inventario: logging off...");
                        }
                    }
                    // In questo caso la lista è una lista filtrata e la sincronizzaiozne non è generale
                    // Quindi aggiorna i record in base al WorkerId passato e all'Id dei record da aggiornare
                    else
                    {
                        var inventarioListUpdated = new List<InventarioDto>();
                        Console.WriteLine($"SyncInventarioRequest: {syncInventarioRequest}");
                        foreach (var item in syncInventarioRequest)
                        {
                            var updatedItem = _inventarioRequestService.UpdateImportedById(new UpdateImportedIdRequestDto
                            {
                                Id = item.InvId,
                                WorkerId = request.WorkerIdSyncRequestDto.WorkerId
                            });
                            inventarioListUpdated.Add(updatedItem);
                        }
                        Console.WriteLine($"InventarioListUpdated: {inventarioListUpdated}");
                    }
                    return inventarioList;
                }
                catch (Exception ex)
                {
                    _logService.AppendErrorToLog($"Error in SyncInventario: {ex.Message}");
                    Console.WriteLine("Error sending data to Mago: logging off..." + ex.Message);

                    throw;
                }
            }
            else
            {
                Console.WriteLine("SyncInventarioList is null or empty: no data to send and therefore ignored");
                return new List<SyncInventarioRequestDto>();
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

        public async Task<(MagoLoginResponseDto? LoginResponse, SettingsDto? Settings)> LoginWithWorkerIdAsync(WorkerIdSyncRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "WorkerId cannot be null or empty");
            }

            var settings = _settingsRepository.GetSettings();
            if (settings == null)
            {
                throw new Exception("Settings not found");
            }

            // Crea il dto per il login
            var magoLoginRequest = settings.ToMagoLoginRequestDto();

            var loginResponse = await LoginAsync(magoLoginRequest);
            return (loginResponse, settings);
        }

        public async Task<MagoLoginResponseDto?> LoginAsync(MagoLoginRequestDto request)
        {
            var response = await _magoApiClient.SendPostAsync("account-manager/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MagoLoginResponseDto>();
                if (result == null)
                {
                    throw new Exception("Login response is null");
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
        
        public async Task<HttpResponseMessage> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, List<PrelMatDto> request)
        {
            if (string.IsNullOrEmpty(responseDto.Token) || request == null)
            {
                throw new ArgumentNullException("ResponseDto, Settings or RequestId cannot be null");
            }

            var settings = _settingsRepository.GetSettings() ?? throw new InvalidOperationException("Settings cannot be null");
            
            var mappedRequest = settings.ToDeleteMoComponentRequestDto(request);

            var response = await _magoApiClient.SendPostAsyncWithToken<DeleteMoComponentRequestDto>(
                "openMes/delete-mo-component",
                mappedRequest,
                responseDto.Token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("SyncPrelMat failed");
            }
            Console.WriteLine($"SyncPrelMat successful response: {response}");

            return response;
        }
    }
}
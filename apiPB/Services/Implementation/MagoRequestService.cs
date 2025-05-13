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

        // Service
        private readonly IMagoAccessService _magoAccessService;

        public MagoRequestService(IMagoApiClient magoApiClient, 
            IMagoAccessService magoAccessService,
            IRegOreRepository regOreRepository,
            IPrelMatRepository prelMatRepository,
            IInventarioRepository inventarioRepository)
        {
            _magoApiClient = magoApiClient;
            _magoAccessService = magoAccessService;
            _regOreRepository = regOreRepository;
            _prelMatRepository = prelMatRepository;
            _inventarioRepository = inventarioRepository;
        }

        public async Task SyncronizeAsync(MagoSyncronizeRequestDto request)
        {
            if(request == null || request.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Login
            var loginResponse = await _magoAccessService.LoginAsync(request);
            if (loginResponse == null)
            {
                throw new Exception("Login failed");
            }

            var token = loginResponse.Token;

            // Recupero Lista di record da A3_app_reg_ore
            var regOreList = _regOreRepository.GetAppRegOre();
            if (regOreList == null || !regOreList.Any())
            {
                throw new Exception("No records found in A3_app_reg_ore");
            }

            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_reg_ore 
            var regOreListUpdated = _regOreRepository.UpdateRegOreImported(request.WorkerId);  

            // Recupero Lista di record da A3_app_prel_mat
            var prelMatList = _prelMatRepository.GetAppPrelMat();
            if (prelMatList == null || !prelMatList.Any())
            {
                throw new Exception("No records found in A3_app_prel_mat");
            }

            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_prel_mat
            var prelMatListUpdated = _prelMatRepository.UpdatePrelMatImported(request.WorkerId);

            // Recupero Lista di record da A3_app_inventario
            
            // Invio Lista di record a Mago

            // Aggiornamento della lista di record in A3_app_inventario

            // Se le operazioni vanno a buon fine, invio Logout

            var logoffRequest = new MagoTokenRequestDto
            {
                Token = token
            };
            try
            {
                await _magoAccessService.LogoffAsync(logoffRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Logoff failed", ex);
            }
        }
    }
}
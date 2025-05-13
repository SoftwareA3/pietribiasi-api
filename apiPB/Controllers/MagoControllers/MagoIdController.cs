using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Services;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using System.Threading.Tasks;

namespace apiPB.MagoApi.Controllers
{
    [Route("api/mago_login")]
    [ApiController]
    public class MagoIdController : ControllerBase
    {
        private readonly IMagoAccessService _magoIdRequestService;
        private readonly bool _isLogActive;

        public MagoIdController( 
            IMagoAccessService magoIdRequestService)
        {
            _magoIdRequestService = magoIdRequestService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetMagoId([FromBody] MagoLoginRequestDto? magoLoginRequestDto)
        {
            if (magoLoginRequestDto == null) return BadRequest("Invalid request");

            var magoIdDto = await _magoIdRequestService.LoginAsync(magoLoginRequestDto);

            if (magoIdDto == null) return NotFound("MagoId not found");

            return Ok(magoIdDto);
        }

        [HttpPost("logoff")]
        public async Task<IActionResult> LogOff([FromBody] MagoTokenRequestDto tokenRequestDto)
        {
            if(tokenRequestDto == null) return BadRequest("Invalid request");

            try
            {
                await _magoIdRequestService.LogoffAsync(tokenRequestDto);
                return Ok("Logoff successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Logoff failed: {ex.Message}");
            }
        }
    }
}
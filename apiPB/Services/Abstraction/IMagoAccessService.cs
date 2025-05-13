using apiPB.Dto.Request;


namespace apiPB.Services.Abstraction
{
    public interface IMagoAccessService
    {
        Task<MagoLoginResponseDto> LoginAsync(MagoLoginRequestDto request);
        Task LogoffAsync(MagoTokenRequestDto requestToken);
    }
}
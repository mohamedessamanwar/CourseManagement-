using BusinessAccessLayer.DTOS.AuthDtos;
namespace BusinessAccessLayer.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthModel> GetTokenAsync(LoginDto model);
        Task<AuthModel> RegisterAsync(RegisterDto model);

    }
}

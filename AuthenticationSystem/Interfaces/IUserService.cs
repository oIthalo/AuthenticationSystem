using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
namespace AuthenticationSystem.Interfaces;

public interface IUserService
{
    Task<ResponseUser> Register(RequestRegister model);
    Task<ResponseLogin> Login(RequestLogin model);
    Task<ResponseUser> GetUserByToken(string tokenJwt);
    Task<ResponseLogin> Refresh(string token, string refreshToken);
    void Logout(string refreshTokenJwt);
    Task<string> ForgotPassword(RequestForgotPassword request);
    Task<string> ResetPassword(RequestResetPassword request);
}
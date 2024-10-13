using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.ValueObjects;
namespace AuthenticationSystem.Interfaces;

public interface IUserService
{
    Task<ResponseUser> Register(RequestRegister model);
    Task<ResponseLogin> Login(RequestLogin model);
    Task<ResponseUser> GetUserByToken(string tokenJwt);
    Task<ResponseLogin> Refresh(string token, string refreshToken);
    void Logout(EmailVO email);
    Task<string> ForgotPassword(RequestForgotPasssword request);
    Task ResetPassword(RequestResetPassword request);
}
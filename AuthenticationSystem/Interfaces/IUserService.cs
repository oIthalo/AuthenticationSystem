using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
namespace AuthenticationSystem.Interfaces;

public interface IUserService
{
    Task<UserResponse> Register(UserRequestRegister model);
    Task<UserResponseLogin> Login(UserRequestLogin model);
    Task<UserResponse> GetUserByToken(string tokenJwt);
    Task<UserResponseLogin> Refresh(string token, string refreshToken);
    void Logout(string username);
    Task<string> ForgotPassword(ForgotPasswordRequest request);
    Task ResetPassword(ResetPasswordRequest request);
}
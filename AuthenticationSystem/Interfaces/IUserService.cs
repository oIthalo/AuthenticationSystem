using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
namespace AuthenticationSystem.Interfaces;

public interface IUserService
{
    Task<UserResponse> Register(UserRequestRegister model);
    Task<UserResponseLogin> Login(UserRequestLogin model);
    UserResponse GetUserByToken(string tokenJwt);
}
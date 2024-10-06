using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
namespace AuthenticationSystem.Services;

public interface IUserService
{
    Task<UserResponse> Register(UserRequest model);
}
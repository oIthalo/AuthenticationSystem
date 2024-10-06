using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Services;
namespace AuthenticationSystem.EndPoints;

public static class UserExtensions
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        app.MapPost("/register", async (IUserService userService, UserRequest model) =>
        {
            var userResponse = await userService.Register(model);
            if (userResponse == null) return Results.BadRequest();

            return Results.Ok(userResponse);
        });
    }
}
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace AuthenticationSystem.EndPoints;

public static class UserExtensions
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("auth").WithTags("Auth");

        groupBuilder.MapGet("/get-user-by-token", (IUserService userService, [FromQuery] string tokenJwt) =>
        {
            var token = userService.GetUserByToken(tokenJwt);
            return Results.Ok(token);
        });

        groupBuilder.MapPost("/user-register", async (IUserService userService, [FromBody] RequestRegister model) =>
        {
            var response = await userService.Register(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/user-login", async (IUserService userService, [FromBody] RequestLogin model) =>
        {
            var response = await userService.Login(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/token-refresh", async (IUserService userService, [FromBody] RequestRefresh request) =>
        {
            var response = await userService.Refresh(request.TokenJwt, request.RefreshToken);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/user-logout", (IUserService userService, [FromQuery] string refreshTokenJwt) =>
        {
            userService.Logout(refreshTokenJwt);
            return Results.Ok();
        });

        groupBuilder.MapPost("/forgot-password", async (IUserService userService, [FromBody] RequestForgotPassword request) =>
        {
            var response = await userService.ForgotPassword(request);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/reset-password", async (IUserService userService, [FromBody] RequestResetPassword request) =>
        {
            var response = await userService.ResetPassword(request);
            return Results.Ok(response);
        });
    }
}
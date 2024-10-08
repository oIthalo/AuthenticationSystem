using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace AuthenticationSystem.EndPoints;

public static class UserExtensions
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("auth").WithTags("Auth");

        groupBuilder.MapGet("/user", (IUserService userService, string tokenJwt) =>
        {
            var token = userService.GetUserByToken(tokenJwt);
            return Results.Ok(token);
        });

        groupBuilder.MapPost("/register", async (IUserService userService, UserRequestRegister model) =>
        {
            var response = await userService.Register(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/login", async (IUserService userService, UserRequestLogin model) =>
        {
            var response = await userService.Login(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/refresh", async (IUserService userService, string token, string refreshToken) =>
        {
            var response = await userService.Refresh(token, refreshToken);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/logout", async (IUserService userService, string username) =>
        {
            userService.Logout(username);
            return Results.Ok();
        });
    }
}
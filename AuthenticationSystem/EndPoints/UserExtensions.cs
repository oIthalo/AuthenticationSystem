using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.ValueObjects;
using Microsoft.AspNetCore.Mvc;
namespace AuthenticationSystem.EndPoints;

public static class UserExtensions
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("auth").WithTags("Auth");

        groupBuilder.MapPost("/user", (IUserService userService, [FromQuery] string tokenJwt) =>
        {
            var token = userService.GetUserByToken(tokenJwt);
            return Results.Ok(token);
        });

        groupBuilder.MapPost("/register", async (IUserService userService, [FromBody] RequestRegister model) =>
        {
            var response = await userService.Register(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/login", async (IUserService userService, [FromBody] RequestLogin model) =>
        {
            var response = await userService.Login(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/refresh", async (IUserService userService, [FromBody] RequestRefresh request) =>
        {
            var response = await userService.Refresh(request.TokenJwt, request.RefreshToken);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/logout", (IUserService userService, [FromBody] EmailVO email) =>
        {
            userService.Logout(email);
            return Results.Ok();
        });

        groupBuilder.MapPost("/forgot-password", async (IUserService userService, [FromBody] RequestForgotPasssword request) =>
        {
            var response = await userService.ForgotPassword(request);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/reset-password", async (IUserService userService, [FromBody] RequestResetPassword request) =>
        {
            await userService.ResetPassword(request);
            return Results.Ok();
        }).RequireAuthorization();
    }
}
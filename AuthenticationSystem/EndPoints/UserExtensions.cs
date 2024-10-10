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
        }).RequireAuthorization("Admin");

        groupBuilder.MapPost("/register", async (IUserService userService, [FromBody] UserRequestRegister model) =>
        {
            var response = await userService.Register(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/login", async (IUserService userService, [FromBody] UserRequestLogin model) =>
        {
            var response = await userService.Login(model);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/refresh", async (IUserService userService, string token, string refreshToken) =>
        {
            var response = await userService.Refresh(token, refreshToken);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/logout", (IUserService userService, [FromBody] string username) =>
        {
            userService.Logout(username);
            return Results.Ok();
        });

        groupBuilder.MapPost("/forgot-password", async (IUserService userService, [FromBody] ForgotPasswordRequest request) =>
        {
            var response = await userService.ForgotPassword(request);
            return Results.Ok(response);
        });

        groupBuilder.MapPost("/reset-password", async (IUserService userService, ResetPasswordRequest request) =>
        {
            await userService.ResetPassword(request);
            return Results.Ok();
        }).RequireAuthorization();
    }
}
using AuthenticationSystem.Data;
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace AuthenticationSystem.EndPoints;

public static class UserExtensions
{
    public static void MapUserEndPoints(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("auth").WithTags("Auth");

        groupBuilder.MapPost("/register", async (IUserService userService, UserRequestRegister model) =>
        {
            var userResponse = await userService.Register(model);
            if (userResponse == null) return Results.BadRequest("Erro ao registrar usuário");
            return Results.Ok(userResponse);
        });

        groupBuilder.MapPost("/login", async (IUserService userService, UserRequestLogin model) =>
        {
            var userResponse = await userService.Login(model);
            if (userResponse == null) return Results.BadRequest("Erro ao logar usuário");
            return Results.Ok(userResponse);
        });
    }
}
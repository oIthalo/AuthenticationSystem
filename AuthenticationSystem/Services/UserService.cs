using AuthenticationSystem.Data;
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AuthenticationSystem.Services;

public class UserService : IUserService
{
    private AppDbContext _context;
    private IMapper _mapper;
    private ITokenService _tokenService;

    public UserService(AppDbContext appDbContext, IMapper mapper, ITokenService tokenService)
    {
        _context = appDbContext;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<UserResponse> Register(UserRequestRegister model)
    {
        User user = _mapper.Map<User>(model);
        if (user is null) throw new Exception("Usuário inválido");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "User");
        if (role is null) throw new Exception("Role não encontrado");

        user.RoleId = role.Id;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.Role = role.Name;
        return userResponse;
    }

    public async Task<UserResponseLogin> Login(UserRequestLogin model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
        if (user == null) throw new Exception("Usuário não encontrado");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
        if (role == null) throw new Exception("Role não encontrado");
        user.Role = role!;

        var token = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        _tokenService.SaveRefreshToken(user.Username, refreshToken);
        return new UserResponseLogin
        {
            Username = user.Username,
            Role = role.Name,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public UserResponse GetUserByToken(string tokenJwt)
    {
        if (string.IsNullOrWhiteSpace(tokenJwt)) throw new ArgumentException("Token JWT não pode ser nulo ou vazio", nameof(tokenJwt));

        var parts = tokenJwt.Split('.');
        if (parts.Length != 3) throw new Exception("Token JWT inválido. O token deve conter três partes.");

        var payload = parts[1];
        if (payload == null) throw new Exception("Payload do token inválido");

        // decodifica o payload de Base64Url
        var decodedPayload = DecodeBase64Url(payload);

        // converte o JSON recebido para um objeto C#
        var userResponse = JsonSerializer.Deserialize<UserResponse>(decodedPayload);

        return userResponse!;
    }

    public async Task<UserResponseLogin> Refresh(string token, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity!.Name;
        var savedRefreshToken = _tokenService.GetRefreshToken(username!);
        if (savedRefreshToken != refreshToken)
            throw new SecurityTokenException("Token inválido");

        var newJwtToken = _tokenService.GenerateToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        _tokenService.DeleteRefreshToken(username!, refreshToken);
        _tokenService.SaveRefreshToken(username!, newRefreshToken);

        var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        var role = roleClaim?.Value ?? "User";

        return new UserResponseLogin
        {
            Username = username!,
            Role = role,
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
        };
    }
    
    private string DecodeBase64Url(string base64Url)
    {
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] data = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(data);
    }
}
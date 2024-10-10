using AuthenticationSystem.Data;
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AuthenticationSystem.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public UserService(AppDbContext appDbContext, IMapper mapper, ITokenService tokenService)
    {
        _context = appDbContext;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<UserResponse> Register(UserRequestRegister model)
    {
        User user = _mapper.Map<User>(model);
        if (user is null) throw new ArgumentNullException(nameof(user), "Usuário inválido. Verifique os dados enviados.");

        var emailExists = await _context.Users.AnyAsync(x => x.Email == model.Email);
        if (emailExists) throw new InvalidOperationException("Este e-mail já está cadastrado.");

        var usernameExists = await _context.Users.AnyAsync(x => x.Username == model.Username);
        if (usernameExists) throw new InvalidOperationException("Este nome de usuário já está cadastrado.");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "User");
        if (role is null) throw new InvalidOperationException("Role 'User' não encontrada. Verifique se a role foi configurada corretamente.");

        var passwordHashed = BCrypt.Net.BCrypt.HashPassword(model.Password);

        user.Password = passwordHashed;
        user.RePassword = passwordHashed;
        user.RoleId = role.Id;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.Role = role.Name;
        return userResponse;
    }

    public async Task<UserResponseLogin> Login(UserRequestLogin model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (user is null) throw new UnauthorizedAccessException("Usuário ou senha incorretos.");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
        if (role is null) throw new InvalidOperationException("Role não encontrada.");

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            throw new UnauthorizedAccessException("Usuário ou senha incorretos.");

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

    public async Task<UserResponse> GetUserByToken(string tokenJwt)
    {
        if (string.IsNullOrWhiteSpace(tokenJwt)) throw new ArgumentException("Token JWT não pode ser nulo ou vazio.", nameof(tokenJwt));

        var parts = tokenJwt.Split('.');
        if (parts.Length != 3) throw new SecurityTokenException("Token JWT inválido. O token deve conter três partes.");

        var payload = parts[1];
        if (payload == null) throw new SecurityTokenException("Payload do token inválido.");

        var decodedPayload = DecodeBase64Url(payload);

        var userResponse = JsonSerializer.Deserialize<UserResponse>(decodedPayload);

        return userResponse ?? throw new InvalidOperationException("Erro ao desserializar o payload do token.");
    }

    public async Task<UserResponseLogin> Refresh(string token, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity?.Name ?? throw new SecurityTokenException("Token inválido. Não foi possível extrair o nome de usuário.");

        var savedRefreshToken = _tokenService.GetRefreshToken(username);
        if (savedRefreshToken != refreshToken)
            throw new SecurityTokenException("Token de atualização inválido.");

        var newJwtToken = _tokenService.GenerateToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.DeleteRefreshToken(username, refreshToken);
        _tokenService.SaveRefreshToken(username, newRefreshToken);

        var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        var role = roleClaim?.Value ?? "User";

        return new UserResponseLogin
        {
            Username = username,
            Role = role,
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
        };
    }

    public void Logout(string username)
    {
        _tokenService.Logout(username);
    }

    public async Task<string> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
        if (user is null) throw new UnauthorizedAccessException("Usuário não encontrado.");

        var token = _tokenService.GenerateRefreshToken();

        var passwordResetToken = new ResetPasswordToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
        };

        await _context.ResetPasswordTokens.AddAsync(passwordResetToken);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var resetToken = await _context.ResetPasswordTokens.FirstOrDefaultAsync(x => x.Token == request.Token && x.Expiration > DateTime.UtcNow && !x.IsUsed);

        if (resetToken == null) throw new UnauthorizedAccessException("Token inválido ou expirado.");

        var user = await _context.Users.FindAsync(resetToken.UserId);
        if (user is null) throw new UnauthorizedAccessException("Usuário não encontrado");

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        resetToken.IsUsed = true;

        if (resetToken.IsUsed)
        {
            _context.ResetPasswordTokens.Remove(resetToken);
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("O token já foi utilizado.");
        }

        if (resetToken.Expiration <= DateTime.UtcNow)
        {
            _context.ResetPasswordTokens.Remove(resetToken);
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("O token já expirou.");
        }

        _context.ResetPasswordTokens.Update(resetToken);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
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
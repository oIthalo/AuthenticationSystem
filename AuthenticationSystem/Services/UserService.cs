using AuthenticationSystem.Data;
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationSystem.Services;

public class UserService : IUserService
{
    private AppDbContext _context;
    private IMapper _mapper;

    public UserService(AppDbContext appDbContext, IMapper mapper)
    {
        _context = appDbContext;
        _mapper = mapper;
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
        userResponse.RoleName = role.Name;
        return userResponse;
    }

    public async Task<UserResponseLogin> Login(UserRequestLogin model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
        if (user == null) throw new Exception("Usuário não encontrado");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
        if (role == null) throw new Exception("Role não encontrado");
        user.Role = role!;

        var token = TokenService.GenerateToken(user);
        return new UserResponseLogin
        {
            Username = user.Username,
            Role = role.Name,
            Token = token
        };
    }

}
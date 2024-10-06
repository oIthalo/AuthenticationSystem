using AuthenticationSystem.Data;
using AuthenticationSystem.Data.DataRequests;
using AuthenticationSystem.Data.DataResponses;
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

    public async Task<UserResponse> Register(UserRequest model)
    {
        User user = _mapper.Map<User>(model);
        if (user is null) throw new Exception("Usuário inválido.");

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "admin");
        if (role is null) throw new Exception("Role não encontrado");

        user.RoleId = role.Id;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.RoleName = role.Name;
        return userResponse;
    }
}
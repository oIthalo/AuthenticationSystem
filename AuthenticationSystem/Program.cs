using AuthenticationSystem.Data;
using AuthenticationSystem.EndPoints;
using AuthenticationSystem.Interfaces;
using AuthenticationSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen().AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

var connectionStr = builder.Configuration.GetConnectionString("AuthenticationConnection");

builder.Services.AddDbContext<AppDbContext>(opts => opts
    .UseMySql(connectionStr, ServerVersion
    .AutoDetect(connectionStr)));

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var key = Encoding.ASCII.GetBytes(secretKey!);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = true,
    };
});

builder.Services.AddAuthorization(opts =>
    {
        opts.AddPolicy("admin", policy => policy
            .RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role &&
            string.Equals(c.Value, "admin",
            StringComparison.OrdinalIgnoreCase)
            )
        )
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.MapUserEndPoints();

app.UseHttpsRedirection();
app.Run();
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MeetAgent.DataAccess;
using MeetAgent.DTOs;
using MeetAgent.Business.Interfaces;
using MeetAgent.Models;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Generators;

namespace MeetAgent.Business.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string?> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<string?> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var secret = _config["JwtSettings:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddDays(int.Parse(_config["JwtSettings:ExpiryDays"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
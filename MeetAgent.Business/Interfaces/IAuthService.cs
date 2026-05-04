using MeetAgent.DTOs;

namespace MeetAgent.Business.Interfaces;

public interface IAuthService
{
    Task<string?> Register(RegisterDto dto);
    Task<string?> Login(LoginDto dto);
}
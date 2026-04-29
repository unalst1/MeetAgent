using Microsoft.AspNetCore.Mvc;
using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;

namespace MeetingAssistantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var token = await _authService.Register(dto);
        if (token == null)
            return BadRequest(new { message = "Bu email zaten kayıtlı." });

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.Login(dto);
        if (token == null)
            return Unauthorized(new { message = "Email veya şifre hatalı." });

        return Ok(new { token });
    }
}
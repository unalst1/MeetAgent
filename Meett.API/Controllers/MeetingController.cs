using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetAgent.DTOs; // DTO'ların yeni yeri
using MeetAgent.Business.Interfaces;

namespace MeetAgent.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingController : ControllerBase
{
    public class TranscriptRequest
    {
        public string Transcript { get; set; }
    }

    private readonly IMeetingService _meetingService;
    private readonly IGeminiService _geminiService; // 1. Gemini servisimizi tanımladık

    // 2. Yapıcı metoda (Constructor) Gemini servisini de enjekte ettik
    public MeetingController(IMeetingService meetingService, IGeminiService geminiService)
    {
        _meetingService = meetingService;
        _geminiService = geminiService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateMeeting(MeetingDto dto)
    {
        var meeting = await _meetingService.CreateMeeting(dto, GetUserId());
        return Ok(meeting);
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetings()
    {
        var meetings = await _meetingService.GetMeetings(GetUserId());
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeeting(int id)
    {
        var meeting = await _meetingService.GetMeetingById(id, GetUserId());
        if (meeting == null) return NotFound();
        return Ok(meeting);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeeting(int id)
    {
        var result = await _meetingService.DeleteMeeting(id, GetUserId());
        if (!result) return NotFound();
        return Ok(new { message = "Toplantı silindi." });
    }

    // 3. İŞTE ŞOV KISMI: Yapay Zeka Endpoint'imiz!
    [HttpPost("extract-tasks")]
    [AllowAnonymous]
    public async Task<IActionResult> ExtractTasks([FromBody] TranscriptRequest request) // string yerine TranscriptRequest yaptık
    {
        // 1. Gelen veri boş mu kontrolü
        if (request == null || string.IsNullOrWhiteSpace(request.Transcript))
            return BadRequest("Toplantı metni boş olamaz.");

        try
        {
            // 2. İşlemi gerçekleştir (Artık request içindeki Transcript'i gönderiyoruz)
            var tasks = await _geminiService.ExtractTasksFromTranscriptAsync(request.Transcript);

            // 3. Sonucu dön
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            // Hata olursa kütüphane sessizliğinde terminalden bakabilmen için mesajı dönüyoruz
            return StatusCode(500, $"Yapay zeka servisi hatası: {ex.Message}");
        }
    }
}
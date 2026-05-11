using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetAgent.DTOs;
using MeetAgent.Business.Interfaces;


namespace MeetAgent.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly IGeminiService _geminiService;
    private readonly ITrelloService _trelloService; // Trello servisini ekledik

    public MeetingController(IMeetingService meetingService, IGeminiService geminiService, ITrelloService trelloService)
    {
        _meetingService = meetingService;
        _geminiService = geminiService;
        _trelloService = trelloService; // Enjekte ettik
    }

    [HttpPost("extract-tasks")]
    [AllowAnonymous]
    public async Task<IActionResult> ExtractTasks([FromBody] string transcript)
    {
        if (string.IsNullOrWhiteSpace(transcript))
            return BadRequest("Toplantı metni boş olamaz.");

        try
        {
            // 1. Gemini'den görevleri listele
            var tasks = await _geminiService.ExtractTasksFromTranscriptAsync(transcript);

            // 2. HER BİR GÖREVİ TRELLO'YA GÖNDER
            // ÖNEMLİ: Trello panondaki "Bugün" listesinin ID'sini buraya yapıştırmalısın.
            // Şimdilik test için statik bir ID verebilirsin.
            string testListId = "6a0233e1e6d9491b3cf61dec";

            foreach (var task in tasks)
            {
                task.TrelloListId = testListId;
                var (cardId, cardUrl) = await _trelloService.CreateCardAsync(task);

                // İstersen her göreve Trello linkini de ekleyebilirsin
                task.Description += $"\n\nTrello Kartı: {cardUrl}";
            }

            return Ok(tasks);
        }
        catch(Exception ex)
{
            // Hatayı gizlemek yerine tüm detayıyla dışarı fırlatıyoruz
            var innerError = ex.InnerException != null ? ex.InnerException.Message : "";
            return StatusCode(500, $"Hata Detayı: {ex.Message} | İç Hata: {innerError} | Stack: {ex.StackTrace}");
        }
    }

    // Diğer metodlar (GetMeetings, CreateMeeting vs.) olduğu gibi kalabilir...
}
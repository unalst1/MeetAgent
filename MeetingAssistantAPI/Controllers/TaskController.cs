using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;

namespace MeetingAssistantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITrelloService _trelloService;

    public TaskController(ITrelloService trelloService)
    {
        _trelloService = trelloService;
    }

    [HttpPost("trello")]
    public async Task<IActionResult> CreateTrelloCard(TaskDto dto)
    {
        try
        {
            var (cardId, cardUrl) = await _trelloService.CreateCard(dto);
            return Ok(new { cardId, cardUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
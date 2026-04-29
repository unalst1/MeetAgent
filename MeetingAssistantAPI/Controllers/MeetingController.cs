using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;

namespace MeetingAssistantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
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
}
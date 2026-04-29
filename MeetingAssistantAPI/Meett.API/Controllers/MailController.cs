using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingAssistantAPI.Models;
using MeetingAssistantAPI.Interfaces;
using MeetingAssistantAPI.DTOs;

namespace MeetingAssistantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMail(MailDto dto)
    {
        try
        {
            await _mailService.SendMail(dto);
            return Ok(new { message = "Mail gönderildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("send-summary")]
    public async Task<IActionResult> SendSummary([FromBody] SendSummaryDto dto)
    {
        try
        {
            await _mailService.SendMeetingSummary(
                dto.ToEmail,
                dto.MeetingTitle,
                dto.Summary,
                dto.Decisions,
                dto.Tasks
            );
            return Ok(new { message = "Özet maili gönderildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class SendSummaryDto
{
    public string ToEmail { get; set; } = string.Empty;
    public string MeetingTitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> Decisions { get; set; } = new();
    public List<string> Tasks { get; set; } = new();
}
namespace MeetingAssistantAPI.DTOs;

public class MailDto
{
    public List<string> ToEmails { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
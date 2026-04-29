using MeetingAssistantAPI.DTOs;

namespace MeetingAssistantAPI.Interfaces;

public interface IMailService
{
    Task SendMail(MailDto dto);
    Task SendMeetingSummary(string toEmail, string meetingTitle, string summary, List<string> decisions, List<string> tasks);
}
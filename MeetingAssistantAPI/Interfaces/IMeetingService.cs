using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Models;

namespace MeetingAssistantAPI.Interfaces;

public interface IMeetingService
{
    Task<Meeting> CreateMeeting(MeetingDto dto, int userId);
    Task<List<Meeting>> GetMeetings(int userId);
    Task<Meeting?> GetMeetingById(int id, int userId);
    Task<bool> DeleteMeeting(int id, int userId);
}
using MeetingAssistantAPI.DTOs;

namespace MeetingAssistantAPI.Interfaces;

public interface ITrelloService
{
    Task<(string cardId, string cardUrl)> CreateCard(TaskDto dto);
}
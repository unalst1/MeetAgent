using MeetAgent.DTOs;

namespace MeetAgent.Business.Interfaces;

public interface ITrelloService
{
    Task<(string cardId, string cardUrl)> CreateCardAsync(TaskDto dto);
}
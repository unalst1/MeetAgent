

using MeetingAssistantAPI.DTOs;

namespace MeetingAssistantAPI.Interfaces
{
    public interface IGeminiService
    {
        Task<List<TaskDto>> ExtractTasksFromTranscriptAsync(string transcript);
    }
}
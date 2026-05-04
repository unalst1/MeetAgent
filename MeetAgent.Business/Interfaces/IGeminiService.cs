

using MeetAgent.DTOs;

namespace MeetAgent.Business.Interfaces
{
    public interface IGeminiService
    {
        Task<List<TaskDto>> ExtractTasksFromTranscriptAsync(string transcript);
    }
}
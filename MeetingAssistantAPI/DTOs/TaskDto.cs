namespace MeetingAssistantAPI.DTOs;

public class TaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string? TrelloBoardId { get; set; }
    public string? TrelloListId { get; set; }
}
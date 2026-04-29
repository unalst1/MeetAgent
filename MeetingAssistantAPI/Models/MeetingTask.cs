namespace MeetingAssistantAPI.Models;

public class MeetingTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string Status { get; set; } = "Todo";
    public string? TrelloCardId { get; set; }
    public string? TrelloCardUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int MeetingId { get; set; }
    public Meeting Meeting { get; set; } = null!;
}
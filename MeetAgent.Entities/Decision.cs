namespace MeetAgent.Models;

public class Decision
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int MeetingId { get; set; }
    public Meeting Meeting { get; set; } = null!;
}
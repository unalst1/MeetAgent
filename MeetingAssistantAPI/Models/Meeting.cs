namespace MeetingAssistantAPI.Models;

public class Meeting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string RawTranscript { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<MeetingTask> Tasks { get; set; } = new List<MeetingTask>();
    public ICollection<Decision> Decisions { get; set; } = new List<Decision>();
}
namespace MeetAgent.DTOs;

public class MeetingDto
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string RawTranscript { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Decisions { get; set; } = new();
    public List<TaskDto> Tasks { get; set; } = new();
}
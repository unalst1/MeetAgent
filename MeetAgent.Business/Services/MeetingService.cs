using Microsoft.EntityFrameworkCore;
using MeetingAssistantAPI.Data;
using MeetingAssistantAPI.DTOs;
using MeetingAssistantAPI.Interfaces;
using MeetingAssistantAPI.Models;

namespace MeetingAssistantAPI.Services;

public class MeetingService : IMeetingService
{
    private readonly AppDbContext _context;
    private readonly ITrelloService _trelloService;

    public MeetingService(AppDbContext context, ITrelloService trelloService)
    {
        _context = context;
        _trelloService = trelloService;
    }

    public async Task<Meeting> CreateMeeting(MeetingDto dto, int userId)
    {
        var meeting = new Meeting
        {
            Title = dto.Title,
            Summary = dto.Summary,
            RawTranscript = dto.RawTranscript,
            MeetingDate = dto.MeetingDate,
            UserId = userId
        };

        foreach (var d in dto.Decisions)
        {
            meeting.Decisions.Add(new Decision { Content = d });
        }

        foreach (var t in dto.Tasks)
        {
            var task = new MeetingTask
            {
                Title = t.Title,
                Description = t.Description,
                AssignedTo = t.AssignedTo
            };

            if (!string.IsNullOrEmpty(t.TrelloListId))
            {
                try
                {
                    var (cardId, cardUrl) = await _trelloService.CreateCard(t);
                    task.TrelloCardId = cardId;
                    task.TrelloCardUrl = cardUrl;
                }
                catch { }
            }

            meeting.Tasks.Add(task);
        }

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return meeting;
    }

    public async Task<List<Meeting>> GetMeetings(int userId)
    {
        return await _context.Meetings
            .Include(m => m.Tasks)
            .Include(m => m.Decisions)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.MeetingDate)
            .ToListAsync();
    }

    public async Task<Meeting?> GetMeetingById(int id, int userId)
    {
        return await _context.Meetings
            .Include(m => m.Tasks)
            .Include(m => m.Decisions)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
    }

    public async Task<bool> DeleteMeeting(int id, int userId)
    {
        var meeting = await _context.Meetings
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meeting == null) return false;

        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync();
        return true;
    }
}
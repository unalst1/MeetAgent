using Microsoft.EntityFrameworkCore;
using MeetingAssistantAPI.Models;

namespace MeetingAssistantAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingTask> MeetingTasks { get; set; }
    public DbSet<Decision> Decisions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Meeting>()
            .HasOne(m => m.User)
            .WithMany(u => u.Meetings)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<MeetingTask>()
            .HasOne(t => t.Meeting)
            .WithMany(m => m.Tasks)
            .HasForeignKey(t => t.MeetingId);

        modelBuilder.Entity<Decision>()
            .HasOne(d => d.Meeting)
            .WithMany(m => m.Decisions)
            .HasForeignKey(d => d.MeetingId);
    }
}
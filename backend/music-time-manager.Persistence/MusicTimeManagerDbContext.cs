using Microsoft.EntityFrameworkCore;
using music_time_manager.Persistence.Configurations;
using music_time_manager.Persistence.Entities;

namespace music_time_manager.Persistence;

public class MusicTimeManagerDbContext : DbContext
{
    public MusicTimeManagerDbContext(DbContextOptions<MusicTimeManagerDbContext> options)
        : base(options)
    {

    }

    public MusicTimeManagerDbContext() { }
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<SubtaskEntity> Subtasks { get; set; }
    public DbSet<TaskAssigneeEntity> TaskAssignees { get; set; }
    public DbSet<SubtaskAssigneeEntity> SubtaskAssignees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Persistence.Entities;

namespace music_time_manager.Persistence.Configurations;

public class TaskAssigneeEntityConfiguration : IEntityTypeConfiguration<TaskAssigneeEntity>
{
    public void Configure(EntityTypeBuilder<TaskAssigneeEntity> builder)
    {
        builder.HasKey(ta => new {ta.TaskId, ta.UserId});
        
        builder.HasOne(ta => ta.Task)
            .WithMany(task => task.TaskAssignees)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.User)
            .WithMany(user => user.TaskAssignees)
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


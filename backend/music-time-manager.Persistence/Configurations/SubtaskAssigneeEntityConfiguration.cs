using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Persistence.Entities;

namespace music_time_manager.Persistence.Configurations;

public class SubtaskAssigneeEntityConfiguration : IEntityTypeConfiguration<SubtaskAssigneeEntity>
{
    public void Configure(EntityTypeBuilder<SubtaskAssigneeEntity> builder)
    {
        builder.HasKey(sta => new {sta.SubtaskId, sta.UserId});
        
        builder.HasOne(sta => sta.Subtask)
            .WithMany(subtask => subtask.SubtaskAssignees)
            .HasForeignKey(sta => sta.SubtaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(sta => sta.User)
            .WithMany(user => user.SubtaskAssignees)
            .HasForeignKey(sta => sta.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
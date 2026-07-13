using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Persistence.Entities;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Persistence.Configurations;

public class SubtaskEntityConfiguration : IEntityTypeConfiguration<SubtaskEntity>
{
    public void Configure(EntityTypeBuilder<SubtaskEntity> builder)
    {
        builder.HasKey(subtask => subtask.Id);

        builder.Property(subtask => subtask.Title)
            .HasMaxLength(Task.MAX_TITLE_LENGTH)
            .IsRequired();

        builder.Property(subtask => subtask.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(subtask => subtask.Task)
            .WithMany(task => task.SubtaskEntities)
            .HasForeignKey(subtask => subtask.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
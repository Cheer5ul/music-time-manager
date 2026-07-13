using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Persistence.Entities;
using Task = music_time_manager.Core.Models.Task;

namespace music_time_manager.Persistence.Configurations;

public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(Task.MIN_TITLE_LENGTH)
            .IsRequired();
        
        builder.Property(task => task.Description)
            .HasMaxLength(Task.MAX_DESCRIPTION_LENGTH)
            .IsRequired();
        
        builder.Property(task => task.DueDate)
            .IsRequired();
        
        builder.Property(task => task.CreatedAt)
            .IsRequired();
        
        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(task => task.CreatedBy)
            .IsRequired();

        builder.Property(task => task.RecreatedFromTaskId);

        builder.HasOne(task => task.RecreatedFromTask)
            .WithMany(task => task.RecreatedTasks)
            .HasForeignKey(task => task.RecreatedFromTaskId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(task => task.SubtaskEntities);
    }
}
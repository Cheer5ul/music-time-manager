using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Core.Models;
using music_time_manager.Persistence.Entities;

namespace music_time_manager.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(user => user.Id);
        
        builder.Property(user => user.UserName)
            .HasColumnName(nameof(UserEntity.UserName))
            .HasMaxLength(User.MAX_USERNAME_LENGTH)
            .IsRequired();
        
        builder.Property(user => user.PasswordHash)
            .HasColumnName(nameof(UserEntity.PasswordHash))
            .IsRequired();
    }
}
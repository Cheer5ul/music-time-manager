using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using music_time_manager.Persistence.Entities;

namespace music_time_manager.Persistence.Configurations;

public class TelegramLinkEntityConfiguration : IEntityTypeConfiguration<TelegramLinkEntity>
{
    public void Configure(EntityTypeBuilder<TelegramLinkEntity> builder)
    {
        builder.HasKey(linkEntity => new {linkEntity.UserId, linkEntity.ChatId});
        
        builder.Property(linkEntity => linkEntity.UserId).IsRequired();
        
        builder.Property(linkEntity => linkEntity.ChatId).IsRequired();
    }
}
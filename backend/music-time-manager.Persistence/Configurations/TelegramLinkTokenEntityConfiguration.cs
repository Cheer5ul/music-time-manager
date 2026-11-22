using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace music_time_manager.Persistence.Configurations;

public class TelegramLinkTokenEntityConfiguration : IEntityTypeConfiguration<TelegramLinkTokenEntity>
{
    public void Configure(EntityTypeBuilder<TelegramLinkTokenEntity> builder)
    {
        builder.HasKey(e => e.UserId);
        
        builder.Property(e => e.Token).IsRequired();
        
        builder.Property(e => e.ExpiresAt).IsRequired();
    }
}
using MediaHub.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaHub.DAL;

public class WatchedMediaConfiguration : IEntityTypeConfiguration<WatchedMedia>
{
    public void Configure(EntityTypeBuilder<WatchedMedia> builder)
    {
        builder.HasKey(wm => wm.Id);
        builder.Property(wm => wm.Id).ValueGeneratedOnAdd();
        builder.Property(wm => wm.MediaId).IsRequired();
        builder.Property(wm => wm.UserId).IsRequired();
        builder.Property(wm => wm.WatchedAt).IsRequired();
        builder.Property(wm => wm.Watched).IsRequired();
        
        builder.HasOne(wm => wm.Media)
            .WithMany()
            .HasForeignKey(wm => wm.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Create an index on MediaId and UserId for faster lookups
        builder.HasIndex(wm => new { wm.MediaId, wm.UserId }).IsUnique();
    }
}
using MediaHub.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaHub.DAL;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.Property(m => m.Name).IsRequired();
        builder.Property(m => m.Path).IsRequired();
        
        builder.Property(m => m.Path).HasConversion(
            v => v.Value,
            v => (RelativePath)v);
        builder.Property(m => m.ThumbnailPath).IsRequired(false);
        builder.Property(m => m.ThumbnailPath).HasConversion(
            v => v == null ? null : v.Value,
            v => v == null ? null : (RelativePath)v);
        builder.Property(m => m.Type).IsRequired();
        builder.HasMany(m => m.Children).WithOne().HasForeignKey(m => m.ParentId);
        builder.HasIndex(m => m.Name);
    }
}
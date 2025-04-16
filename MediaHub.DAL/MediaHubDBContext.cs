using MediaHub.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.DAL;

public class MediaHubDBContext: DbContext
{
    public DbSet<Media> Media { get; set; }

    public MediaHubDBContext(DbContextOptions<MediaHubDBContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MediaConfiguration());
    }
}
using MediaHub.DAL.Model;

namespace MediaHub.API.Models;

public class MediaWithWatchStatusViewModel
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; }
    public RelativePath Path { get; set; }
    public RelativePath? ThumbnailPath { get; set; }
    public MediaType Type { get; set; }
    public bool IsWatched { get; set; }
    
    // Create from Media entity with watch status
    public static MediaWithWatchStatusViewModel FromMedia(Media media, bool isWatched)
    {
        return new MediaWithWatchStatusViewModel
        {
            Id = media.Id,
            ParentId = media.ParentId,
            Name = media.Name,
            Path = media.Path,
            ThumbnailPath = media.ThumbnailPath,
            Type = media.Type,
            IsWatched = isWatched
        };
    }
}
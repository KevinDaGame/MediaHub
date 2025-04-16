using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IMediaThumbnailService
{
    public byte[]? GetThumbnail(RelativePath path);
    public RelativePath? GetThumbnailPath(RelativePath path);
    
    public Task ExtractThumbnail(RelativePath path);
    
    public void ExtractThumbnailsForMediaFolder();
    
    public void DeleteThumbnail(RelativePath path);
    public void DeleteThumbnailsForPath(RelativePath path);
}
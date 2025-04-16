using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services;

public interface IMediaThumbnailService
{
    public byte[]? GetThumbnail(RelativePath path);
    public RelativePath? GetThumbnailPath(RelativePath path);
    
    public Task ExtractThumbnail(RelativePath path);
    
    public void ExtractThumbnailsForMediaFolder();
    
    public void DeleteThumbnail(RelativePath path);
    public void DeleteThumbnailsForPath(RelativePath path);
}
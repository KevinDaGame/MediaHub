using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services;

public interface IMediaService
{
    public IEnumerable<Media> GetMedia();
    public IEnumerable<Media> GetMedia(Guid id);
    public FileInfo? GetMediaFile(RelativePath path);
}
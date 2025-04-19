using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IMediaService
{
    public Media? GetMediaItem(Guid id);
    public IEnumerable<Media> GetMedia();
    public IEnumerable<Media> GetMedia(Guid id);
    public FileInfo? GetMediaFile(Guid id);

    public List<Media> GetBreadCrumb(Guid? mediaId);
}
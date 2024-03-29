using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services;

public interface IMediaService
{
    public IEnumerable<IMedia> GetMedia();
    public IEnumerable<IMedia> GetMedia(string path);
    public FileInfo? GetMediaFile(string path);
}
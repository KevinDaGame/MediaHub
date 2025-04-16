using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services;

public interface IMediaDiscoveryService
{
    public void DiscoverMedia();
    public IEnumerable<Media> DiscoverMedia(string path, Guid? parentId = null);
}
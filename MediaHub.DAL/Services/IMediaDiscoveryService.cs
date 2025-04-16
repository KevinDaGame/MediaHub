using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IMediaDiscoveryService
{
    public void DiscoverMedia();
    public IEnumerable<Media> DiscoverMedia(string path, Guid? parentId = null);
}
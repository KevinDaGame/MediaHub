using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IMediaDiscoveryService
{
    public void DiscoverMedia();
    public int DiscoverMedia(string path, Guid? parentId = null);
}
using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IMediaDiscoveryService
{
    public void DiscoverMedia();
    public int DiscoverMedia(AbsolutePath path, Guid? parentId = null);
}
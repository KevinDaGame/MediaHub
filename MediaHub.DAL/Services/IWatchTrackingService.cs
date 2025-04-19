using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services;

public interface IWatchTrackingService
{
    IEnumerable<WatchedMedia> GetWatchedMediaByUser(string userId);
    WatchedMedia? GetWatchedMediaByUserAndMediaId(string userId, Guid mediaId);
    void MarkMediaAsWatched(Guid mediaId, string userId);
    void UnmarkMediaAsWatched(Guid mediaId, string userId);
    bool IsMediaWatched(Guid mediaId, string userId);
}
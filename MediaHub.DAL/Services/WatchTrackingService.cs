using MediaHub.DAL.Model;
using MediaHub.DAL.Repository;

namespace MediaHub.DAL.Services;

public class WatchTrackingService : IWatchTrackingService
{
    private readonly WatchedMediaRepository _watchedMediaRepository;

    public WatchTrackingService(WatchedMediaRepository watchedMediaRepository)
    {
        _watchedMediaRepository = watchedMediaRepository;
    }

    public IEnumerable<WatchedMedia> GetWatchedMediaByUser(string userId)
    {
        return _watchedMediaRepository.GetAllWatchedMediaByUser(userId);
    }

    public WatchedMedia? GetWatchedMediaByUserAndMediaId(string userId, Guid mediaId)
    {
        return _watchedMediaRepository.GetWatchedMediaByUserAndMediaId(userId, mediaId);
    }

    public void MarkMediaAsWatched(Guid mediaId, string userId)
    {
        _watchedMediaRepository.MarkMediaAsWatched(mediaId, userId);
    }

    public void UnmarkMediaAsWatched(Guid mediaId, string userId)
    {
        _watchedMediaRepository.UnmarkMediaAsWatched(mediaId, userId);
    }

    public bool IsMediaWatched(Guid mediaId, string userId)
    {
        var watchedMedia = _watchedMediaRepository.GetWatchedMediaByUserAndMediaId(userId, mediaId);
        return watchedMedia != null && watchedMedia.Watched;
    }
}
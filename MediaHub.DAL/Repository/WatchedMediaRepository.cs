using MediaHub.DAL.Model;

namespace MediaHub.DAL.Repository;

public class WatchedMediaRepository
{
    private readonly MediaHubDBContext _context;

    public WatchedMediaRepository(MediaHubDBContext context)
    {
        _context = context;
    }

    public IEnumerable<WatchedMedia> GetAllWatchedMediaByUser(string userId)
    {
        return _context.WatchedMedia
            .Where(wm => wm.UserId == userId)
            .ToList();
    }

    public WatchedMedia? GetWatchedMediaByUserAndMediaId(string userId, Guid mediaId)
    {
        return _context.WatchedMedia
            .FirstOrDefault(wm => wm.UserId == userId && wm.MediaId == mediaId);
    }

    public void MarkMediaAsWatched(Guid mediaId, string userId)
    {
        var existingRecord = GetWatchedMediaByUserAndMediaId(userId, mediaId);
        
        if (existingRecord != null)
        {
            existingRecord.WatchedAt = DateTime.UtcNow;
            existingRecord.Watched = true;
            _context.WatchedMedia.Update(existingRecord);
        }
        else
        {
            var watchedMedia = new WatchedMedia
            {
                Id = Guid.NewGuid(),
                MediaId = mediaId,
                UserId = userId,
                WatchedAt = DateTime.UtcNow,
                Watched = true
            };
            _context.WatchedMedia.Add(watchedMedia);
        }
        
        _context.SaveChanges();
    }
    
    public void UnmarkMediaAsWatched(Guid mediaId, string userId)
    {
        var watchedMedia = GetWatchedMediaByUserAndMediaId(userId, mediaId);
        
        if (watchedMedia != null)
        {
            watchedMedia.Watched = false;
            _context.WatchedMedia.Update(watchedMedia);
            _context.SaveChanges();
        }
    }
}
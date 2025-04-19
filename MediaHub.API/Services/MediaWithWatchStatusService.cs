using MediaHub.API.Models;
using MediaHub.DAL.Services;
using System.Security.Claims;

namespace MediaHub.API.Services;

public class MediaWithWatchStatusService
{
    private readonly IMediaService _mediaService;
    private readonly IWatchTrackingService _watchTrackingService;

    public MediaWithWatchStatusService(IMediaService mediaService, IWatchTrackingService watchTrackingService)
    {
        _mediaService = mediaService;
        _watchTrackingService = watchTrackingService;
    }

    public IEnumerable<MediaWithWatchStatusViewModel> GetMediaWithWatchStatus(Guid? id, ClaimsPrincipal user)
    {
        // Get the base media items
        var mediaItems = id.HasValue 
            ? _mediaService.GetMedia(id.Value) 
            : _mediaService.GetMedia();
        
        // If the user is not authenticated, return items without watch status
        if (user?.Identity?.IsAuthenticated != true)
        {
            return mediaItems.Select(m => MediaWithWatchStatusViewModel.FromMedia(m, false));
        }
        
        // Get the user ID
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                        user.FindFirstValue("sub") ?? 
                        "anonymous";

        // Transform to view models with watch status
        return mediaItems.Select(media => 
        {
            bool isWatched = false;
            
            // Only check watch status for files, not directories
            if (media.Type == MediaHub.DAL.Model.MediaType.FILE)
            {
                isWatched = _watchTrackingService.IsMediaWatched(media.Id, userId);
            }
            
            return MediaWithWatchStatusViewModel.FromMedia(media, isWatched);
        });
    }
}
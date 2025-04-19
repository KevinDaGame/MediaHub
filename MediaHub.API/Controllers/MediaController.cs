using MediaHub.DAL.Model;
using MediaHub.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;

namespace MediaHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly ILogger<MediaController> _logger;
    private readonly IMediaService _mediaService;
    private readonly IMediaThumbnailService _mediaThumbnailService;
    private readonly IWatchTrackingService _watchTrackingService;

    public MediaController(
        ILogger<MediaController> logger, 
        IMediaService mediaService, 
        IMediaThumbnailService mediaThumbnailService,
        IWatchTrackingService watchTrackingService)
    {
        _logger = logger;
        _mediaService = mediaService;
        _mediaThumbnailService = mediaThumbnailService;
        _watchTrackingService = watchTrackingService;
    }

    [HttpGet]
    [Authorize("read:media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<Media> GetMedia([FromQuery] Guid? id)
    {
        return id.HasValue ? _mediaService.GetMedia(id.Value) : _mediaService.GetMedia();
    }

    [HttpGet]
    [Authorize("read:media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("file")]
    public IActionResult GetMediaFile([FromQuery] Guid id)
    {
        var file = _mediaService.GetMediaFile(id);
        if (file == null)
        {
            return NotFound();
        }
        
        // Mark media as watched for current user
        if (User.Identity?.IsAuthenticated == true)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                            User.FindFirstValue("sub") ?? 
                            "anonymous";
            
            _watchTrackingService.MarkMediaAsWatched(id, userId);
            _logger.LogInformation($"Media {id} marked as watched for user {userId}");
        }
        
        //read stream
        new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out var contentType);
        
        var result = new FileStreamResult(file.OpenRead(), contentType ?? "application/octet-stream");
        
        Response.Headers["Content-Disposition"] = "inline; filename=" + file.Name;
        
        return result;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("thumbnail")]
    public IActionResult GetThumbnail([FromQuery] Guid id)
    {
        Media? media = _mediaService.GetMediaItem(id);
        if (media == null)
        {
            return NotFound();
        }
        var thumbnail = _mediaThumbnailService.GetThumbnail(media);
        
        return File(thumbnail, "image/webp");
    }
    
    [HttpGet]
    [Authorize("read:media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("watched")]
    public IActionResult GetWatchedMedia()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                        User.FindFirstValue("sub") ?? 
                        "anonymous";
        
        var watchedMedia = _watchTrackingService.GetWatchedMediaByUser(userId);
        return Ok(watchedMedia);
    }
    
    [HttpPost]
    [Authorize("read:media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("watched/{id}")]
    public IActionResult MarkMediaAsWatched(Guid id)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                        User.FindFirstValue("sub") ?? 
                        "anonymous";
        
        _watchTrackingService.MarkMediaAsWatched(id, userId);
        return Ok();
    }
    
    [HttpDelete]
    [Authorize("read:media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("watched/{id}")]
    public IActionResult UnmarkMediaAsWatched(Guid id)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }
        
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                        User.FindFirstValue("sub") ?? 
                        "anonymous";
        
        _watchTrackingService.UnmarkMediaAsWatched(id, userId);
        return Ok();
    }
}
using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Repository;
using MediaHub.DAL.Services.MediaPath;

namespace MediaHub.DAL.Services;

public class MediaService : IMediaService
{
    private readonly IMediaPathService _mediaPathService;
    private readonly IMediaThumbnailService _mediaThumbnailService;
    private readonly IFileSystem _fileSystem;
    private readonly MediaRepository _mediaRepository;

    public MediaService(RootPathService mediaPathService, IMediaThumbnailService mediaThumbnailService,
        IFileSystem fileSystem, MediaRepository mediaRepository)
    {
        _mediaPathService = mediaPathService;
        _mediaThumbnailService = mediaThumbnailService;
        _fileSystem = fileSystem;
        _mediaRepository = mediaRepository;
    }

    public MediaService(RootPathService mediaPathService, IMediaThumbnailService mediaThumbnailService,
        MediaRepository mediaRepository) : this(
        mediaPathService, mediaThumbnailService, new FileSystem(), mediaRepository)
    {
    }

    public IEnumerable<Media> GetMedia()
    {
        return _mediaRepository.getSubMedia(null);
    }

    public IEnumerable<Media> GetMedia(Guid id)
    {
        return _mediaRepository.getSubMedia(id)
            .OrderBy(it => it.Type)
            .ThenBy(it => it.ExtractNumericValueFromName())
            .ThenBy(it => it.Name);
    }

    public FileInfo? GetMediaFile(Guid id)
    {
        Media? media = _mediaRepository.GetMediaById(id);
        return media != null ? new FileInfo(media.Path) : null;
    }

    public List<Media> GetBreadCrumb(Guid? mediaId)
    {
        var breadCrumb = new List<Media>();

        while (mediaId != null)
        {
            Media? media = _mediaRepository.GetMediaById(mediaId.Value);
            if (media == null) break;

            breadCrumb.Add(media);
            mediaId = media.ParentId;
        }

        breadCrumb.Reverse();
        return breadCrumb;
    }
}
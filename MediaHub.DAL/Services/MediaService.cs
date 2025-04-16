using System.IO.Abstractions;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.FS.Repository;
using MediaHub.DAL.FS.Services.MediaPath;

namespace MediaHub.DAL.FS.Services;

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

    public MediaService(RootPathService mediaPathService, IMediaThumbnailService mediaThumbnailService, MediaRepository mediaRepository) : this(
        mediaPathService, mediaThumbnailService, new FileSystem(), mediaRepository)
    {
    }

    public IEnumerable<Media> GetMedia()
    {
        return _mediaRepository.getSubMedia(null);
    }

    public IEnumerable<Media> GetMedia(Guid id)
    {
        return _mediaRepository.getSubMedia(id);
    }

    public FileInfo? GetMediaFile(RelativePath path)
    {
        AbsolutePath fullPath = _mediaPathService.CombineRootPath(path);
        return _fileSystem.File.Exists(fullPath) ? new FileInfo(fullPath) : null;
    }
}
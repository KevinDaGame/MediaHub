using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Repository;
using MediaHub.DAL.Services.MediaPath;

namespace MediaHub.DAL.Services;

public class MediaDiscoveryService : IMediaDiscoveryService
{
    private readonly MediaRepository _mediaRepository;
    private readonly IFileSystem _fileSystem;
    private readonly RootPathService _mediaPathService;

    public MediaDiscoveryService(MediaRepository mediaRepository, IFileSystem fileSystem,
        RootPathService mediaPathService)
    {
        _mediaRepository = mediaRepository;
        _fileSystem = fileSystem;
        _mediaPathService = mediaPathService;
    }

    public MediaDiscoveryService(MediaRepository mediaRepository, RootPathService mediaPathService) : this(
        mediaRepository, new FileSystem(), mediaPathService)
    {
    }

    public void DiscoverMedia()
    {
        Console.WriteLine("Discovering media");
        CleanupOldMedia();

        AbsolutePath rootPath = _mediaPathService.GetAbsolutePath();
        int discoveredMediaCount = DiscoverMedia(rootPath);

        Console.WriteLine($"Discovered {discoveredMediaCount} media items");
    }

    private void CleanupOldMedia()
    {
        Console.WriteLine("Cleaning up old media");
        IEnumerable<Media> media = _mediaRepository.GetAllMediaQuery();
        List<Media> mediaToDelete = media.Where(mediaItem =>
            mediaItem.Type == MediaType.DIRECTORY
                ? !_fileSystem.Directory.Exists(_mediaPathService.CombineRootPath(mediaItem.Path))
                : !_fileSystem.File.Exists(_mediaPathService.CombineRootPath(mediaItem.Path))).ToList();

        _mediaRepository.DeleteMediaMultiple(mediaToDelete);

        Console.WriteLine($"Deleted {mediaToDelete.Count} media items");
    }


    public int DiscoverMedia(AbsolutePath path, Guid? parentId = null)
    {
        var discoveredMediaCount = 0;

        List<RelativePath> paths = _fileSystem.Directory.GetFileSystemEntries(path)
            .Select(file => (AbsolutePath)file)
            .Select(_mediaPathService.StripRootPath)
            .ToList();

        List<Media> media = _mediaRepository.GetMediaByPathMultiple(paths);

        foreach (RelativePath p in paths)
        {
            AbsolutePath mediaPath = _mediaPathService.CombineRootPath(p);
            Media? mediaItem = media.FirstOrDefault(m => m.Path == p);
            if (mediaItem == null)
            {
                mediaItem = new Media
                {
                    Id = Guid.NewGuid(),
                    Path = p,
                    Name = _fileSystem.Path.GetFileName(p),
                    Type = _fileSystem.Directory.Exists(mediaPath) ? MediaType.DIRECTORY : MediaType.FILE,
                    ParentId = parentId
                };
                discoveredMediaCount++;
                _mediaRepository.AddMedia(mediaItem);
            }
            if (_fileSystem.Directory.Exists(mediaPath))
            {
                discoveredMediaCount += DiscoverMedia(mediaPath, mediaItem.Id);
            }
        }
        return discoveredMediaCount;
    }
}
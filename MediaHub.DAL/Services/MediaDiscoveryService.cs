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

        string rootPath = _mediaPathService.GetAbsolutePath();
        List<Media> media = DiscoverMedia(rootPath).ToList();

        _mediaRepository.AddMediaMultiple(media);
        
        Console.WriteLine($"Discovered {media.Count} media items");
    }

    private void CleanupOldMedia()
    {
        Console.WriteLine("Cleaning up old media");
        IEnumerable<Media> media = _mediaRepository.GetAllMediaQuery();
        List<Media> mediaToDelete = media.Where(mediaItem => mediaItem.Type == MediaType.DIRECTORY ? !_fileSystem.Directory.Exists(mediaItem.Path) : !_fileSystem.File.Exists(mediaItem.Path)).ToList();

        _mediaRepository.DeleteMediaMultiple(mediaToDelete);
        
        Console.WriteLine($"Deleted {mediaToDelete.Count} media items");
    }


    public IEnumerable<Media> DiscoverMedia(string path, Guid? parentId = null)
    {
        List<Media> mediaToAdd = new();
        foreach (AbsolutePath entry in _fileSystem.Directory.GetFileSystemEntries(path).Select(file => (AbsolutePath) file))
        {
            RelativePath cleanPath = _mediaPathService.StripRootPath(entry);
            Media? media = _mediaRepository.GetMediaByPath(cleanPath);
            Guid mediaId = media?.Id ?? Guid.NewGuid();
            if (media == null)
            {
                mediaToAdd.Add(new Media
                {
                    Id = mediaId,
                    Path = cleanPath,
                    Name = _fileSystem.Path.GetFileName(entry),
                    Type = _fileSystem.Directory.Exists(entry) ? MediaType.DIRECTORY : MediaType.FILE,
                    ParentId = parentId ?? null
                });
            }

            if (_fileSystem.Directory.Exists(entry))
            {
                mediaToAdd.AddRange(DiscoverMedia(entry, mediaId));
            }
        }

        return mediaToAdd;
    }
}
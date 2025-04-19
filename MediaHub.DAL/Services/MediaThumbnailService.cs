using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Repository;
using MediaHub.DAL.Services.MediaPath;
using MediaHub.DAL.Services.Thumbnail;
using Microsoft.Extensions.DependencyInjection;

namespace MediaHub.DAL.Services;

public class MediaThumbnailService : IMediaThumbnailService
{
    private readonly RootPathService _rootPath;
    private readonly ThumbnailPathService _thumbnailPath;
    private readonly ThumbnailContext _thumbnailContext;
    private readonly IFileSystem _fileSystem;
    private readonly IServiceProvider _serviceProvider;

    public MediaThumbnailService(RootPathService rootPath, ThumbnailPathService thumbnailPath,
        ThumbnailContext thumbnailContext, IFileSystem fileSystem, IServiceProvider serviceProvider)
    {
        _rootPath = rootPath;
        _thumbnailPath = thumbnailPath;
        _thumbnailContext = thumbnailContext;
        _fileSystem = fileSystem;
        _serviceProvider = serviceProvider;
    }

    public MediaThumbnailService(RootPathService rootPath, ThumbnailPathService thumbnailPath,
        ThumbnailContext thumbnailContext, IServiceProvider serviceProvider) : this(rootPath,
        thumbnailPath,
        thumbnailContext,
        new FileSystem(),
        serviceProvider)
    {
    }

    public byte[]? GetThumbnail(Media media)
    {
        if (media.ThumbnailPath == null)
        {
            return null;
        }
        AbsolutePath thumbnailPath = _thumbnailPath.CombineRootPath(media.ThumbnailPath);
        return _fileSystem.File.Exists(thumbnailPath) ? _fileSystem.File.ReadAllBytes(thumbnailPath) : null;
    }

    public async Task ExtractThumbnail(RelativePath path)
    {
        await _thumbnailContext.ExtractThumbnail(path);
    }

    public void ExtractThumbnailsForMediaFolder()
    {
        using var scope = _serviceProvider.CreateScope();
        var mediaRepository = scope.ServiceProvider.GetRequiredService<MediaRepository>();

        var media = mediaRepository.GetAllMediaQuery()
            .Where(mediaItem =>
                mediaItem.Type == MediaType.FILE && mediaItem.ThumbnailPath == null)
            .Where(mediaItem => _thumbnailContext.SupportedExtensions.Contains(mediaItem.Name.Split('.')[^1]))
            .ToList();

        foreach (var mediaItem in media)
        {
            try
            {
                ExtractThumbnail(mediaItem.Path).Wait();
                // Update the media item with the thumbnail path
                mediaItem.ThumbnailPath = mediaItem.Path + ".webp";
                mediaRepository.UpdateMedia(mediaItem);
                Console.WriteLine($"Extracted thumbnail for {mediaItem.Path}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to extract thumbnail for {mediaItem.Path}: {e.Message}");
            }
        }
    }

    public void DeleteThumbnail(RelativePath path)
    {
        AbsolutePath thumbnailPath = _thumbnailPath.CombineRootPath(path + ".webp");
        if (_fileSystem.File.Exists(thumbnailPath))
        {
            _fileSystem.File.Delete(thumbnailPath);
            Console.WriteLine($"Deleted thumbnail for {path}");
        }
        else
        {
            Console.WriteLine($"Thumbnail for {path} does not exist");
        }
    }

    public void DeleteThumbnailsForPath(RelativePath path)
    {
        AbsolutePath thumbnailPath = _thumbnailPath.CombineRootPath(path);
        if (_fileSystem.Directory.Exists(thumbnailPath))
        {
            _fileSystem.Directory.Delete(thumbnailPath, true);
            Console.WriteLine($"Deleted thumbnails for {path}");
        }
        else
        {
            Console.WriteLine($"Thumbnails for {path} do not exist");
        }
    }
}

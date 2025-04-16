using System.IO.Abstractions;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.FS.Services.MediaPath;
using MediaHub.DAL.FS.Services.Thumbnail;
using Xabe.FFmpeg;

namespace MediaHub.DAL.FS.Services;

public class MediaThumbnailService : IMediaThumbnailService
{
    private readonly RootPathService _rootPath;
    private readonly ThumbnailPathService _thumbnailPath;
    private readonly ThumbnailContext _thumbnailContext;
    private readonly IFileSystem _fileSystem;

    public MediaThumbnailService(RootPathService rootPath, ThumbnailPathService thumbnailPath,
        ThumbnailContext thumbnailContext, IFileSystem fileSystem)
    {
        _rootPath = rootPath;
        _thumbnailPath = thumbnailPath;
        _thumbnailContext = thumbnailContext;
        _fileSystem = fileSystem;
    }

    public MediaThumbnailService(RootPathService rootPath, ThumbnailPathService thumbnailPath,
        ThumbnailContext thumbnailContext) : this(rootPath,
        thumbnailPath,
        thumbnailContext,
        new FileSystem())
    {
    }

    public byte[]? GetThumbnail(RelativePath path)
    {
        AbsolutePath thumbnailPath = _thumbnailPath.CombineRootPath(path + ".webp");
        return _fileSystem.File.Exists(thumbnailPath) ? _fileSystem.File.ReadAllBytes(thumbnailPath) : null;
    }

    public RelativePath? GetThumbnailPath(RelativePath path)
    {
        AbsolutePath thumbnailPath = _thumbnailPath.CombineRootPath(path + ".webp");
        return _fileSystem.File.Exists(thumbnailPath) ? path : null;
    }

    public async Task ExtractThumbnail(RelativePath path)
    {
        await _thumbnailContext.ExtractThumbnail(path);
    }

    public void ExtractThumbnailsForMediaFolder()
    {
        var mediaFiles = _fileSystem.Directory.GetFiles(_rootPath.Path, "*.*", SearchOption.AllDirectories)
            .Select(file => (AbsolutePath)file)
            .Select(_rootPath.StripRootPath)
            .Where(file =>
                _thumbnailContext.SupportedExtensions.Contains(_fileSystem.Path.GetExtension(file).TrimStart('.')))
            .Where(file => !_fileSystem.File.Exists(_thumbnailPath.CombineRootPath(file + ".webp")));

        foreach (var mediaFile in mediaFiles)
        {
            try
            {
                ExtractThumbnail(mediaFile).Wait();
                Console.WriteLine($"Extracted thumbnail for {mediaFile}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to extract thumbnail for {mediaFile}: {e.Message}");
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
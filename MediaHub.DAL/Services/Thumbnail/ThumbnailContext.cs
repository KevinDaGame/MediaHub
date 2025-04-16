using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services.MediaPath;

namespace MediaHub.DAL.Services.Thumbnail;

public class ThumbnailContext
{
    private readonly Dictionary<string, IThumbnailStrategy> _strategies = new();

    public ThumbnailContext(RootPathService rootPathService, ThumbnailPathService thumbnailPathService,
        IFileSystem fileSystem)
    {
        var strategies = new List<IThumbnailStrategy>
        {
            new VideoThumbnailStrategy(rootPathService, thumbnailPathService, fileSystem),
            new EpubThumbnailStrategy(rootPathService, thumbnailPathService, fileSystem),
            new CBZThumbnailStrategy(rootPathService, thumbnailPathService, fileSystem)
        };
        
        foreach (IThumbnailStrategy strategy in strategies)
        {
            foreach (string extension in strategy.SupportedExtensions)
            {
                _strategies.Add(extension, strategy);
            }
        }
    }

    public ThumbnailContext(RootPathService rootPathService, ThumbnailPathService thumbnailPathService) : this(
        rootPathService, thumbnailPathService, new FileSystem())
    {
    }

    public Task ExtractThumbnail(RelativePath path)
    {
        string extension = Path.GetExtension(path).TrimStart('.');

        if (_strategies.TryGetValue(extension, out IThumbnailStrategy strategy))
        {
            return strategy.ExtractThumbnail(path);
        }

        throw new NotSupportedException($"No thumbnail strategy found for {extension}");
    }
    
    public IEnumerable<string> SupportedExtensions => _strategies.Keys;
}
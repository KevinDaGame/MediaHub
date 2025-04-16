using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services.MediaPath;

namespace MediaHub.DAL.Services.Thumbnail;

public abstract class ThumbnailStrategy: IThumbnailStrategy
{
    protected readonly RootPathService RootPath;
    protected readonly ThumbnailPathService ThumbnailPath;
    protected readonly IFileSystem FileSystem;

    protected ThumbnailStrategy(RootPathService rootPath, ThumbnailPathService thumbnailPath, IFileSystem fileSystem)
    {
        RootPath = rootPath;
        ThumbnailPath = thumbnailPath;
        FileSystem = fileSystem;
    }
    
    public abstract IEnumerable<string> SupportedExtensions { get; }

    public abstract Task ExtractThumbnail(RelativePath path);
}
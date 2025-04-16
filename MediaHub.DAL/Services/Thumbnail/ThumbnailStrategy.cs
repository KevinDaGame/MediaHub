using System.IO.Abstractions;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.FS.Services.MediaPath;

namespace MediaHub.DAL.FS.Services.Thumbnail;

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
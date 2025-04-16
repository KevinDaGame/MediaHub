using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services.Thumbnail;

public interface IThumbnailStrategy
{
    public Task ExtractThumbnail(RelativePath path);
    IEnumerable<string> SupportedExtensions { get; }
}
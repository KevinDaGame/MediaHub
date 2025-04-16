using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services.Thumbnail;

public interface IThumbnailStrategy
{
    public Task ExtractThumbnail(RelativePath path);
    IEnumerable<string> SupportedExtensions { get; }
}
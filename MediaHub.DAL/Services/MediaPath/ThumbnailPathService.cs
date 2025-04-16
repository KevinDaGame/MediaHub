using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services.MediaPath;

public class ThumbnailPathService: MediaPathService
{
    public ThumbnailPathService(AbsolutePath rootPath) : base(rootPath)
    {
    }
}
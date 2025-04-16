using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services.MediaPath;

public class ThumbnailPathService: MediaPathService
{
    public ThumbnailPathService(AbsolutePath rootPath) : base(rootPath)
    {
    }
}
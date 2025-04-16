using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services.MediaPath;

public class RootPathService: MediaPathService
{
    public RootPathService(AbsolutePath rootPath) : base(rootPath)
    {
    }
}
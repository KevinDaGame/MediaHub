using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services.MediaPath;

public class RootPathService: MediaPathService
{
    public RootPathService(AbsolutePath rootPath) : base(rootPath)
    {
    }
}
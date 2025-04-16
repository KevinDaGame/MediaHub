using MediaHub.DAL.FS.Model;

namespace MediaHub.DAL.FS.Services.MediaPath;

public interface IMediaPathService
{
    RelativePath StripRootPath(AbsolutePath path);
    AbsolutePath CombineRootPath(RelativePath path);
}
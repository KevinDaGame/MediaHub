using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services.MediaPath;

public interface IMediaPathService
{
    RelativePath StripRootPath(AbsolutePath path);
    AbsolutePath CombineRootPath(RelativePath path);
}
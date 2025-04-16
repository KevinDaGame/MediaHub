using System.IO.Abstractions;
using MediaHub.DAL.Model;

namespace MediaHub.DAL.Services.MediaPath;

public abstract class MediaPathService : IMediaPathService
{
    public AbsolutePath Path { get; private set; }
    private readonly IFileSystem _fileSystem;

    public MediaPathService(AbsolutePath path, IFileSystem fileSystem)
    {
        Path = path;
        _fileSystem = fileSystem;

        if (!_fileSystem.Directory.Exists(Path))
        {
            Console.WriteLine($"Path {Path} does not exist. Creating it.");
            _fileSystem.Directory.CreateDirectory(Path);
        }
    }

    public MediaPathService(AbsolutePath rootPath) : this(rootPath, new FileSystem())
    {
    }

    public RelativePath StripRootPath(AbsolutePath path)
    {
        string newPath = ((string)path).Replace(Path, "");
        newPath = newPath.StartsWith(_fileSystem.Path.DirectorySeparatorChar) ? newPath[1..] : newPath;
        return (RelativePath)newPath.Replace("\\", "/");
    }

    public AbsolutePath CombineRootPath(RelativePath path)
    {
        return (AbsolutePath) _fileSystem.Path.Combine(Path, path);
    }

    public AbsolutePath GetAbsolutePath()
    {
        return (AbsolutePath) _fileSystem.Path.GetFullPath(Path);
    }
}
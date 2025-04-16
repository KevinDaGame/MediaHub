using System.IO.Abstractions;
using MediaHub.DAL.FS.Model;
using MediaHub.DAL.FS.Services;
using MediaHub.DAL.FS.Services.MediaPath;

namespace MediaHub.API.Service;

public class MediaFileSystemWatcher
{
    private readonly FileSystemWatcher _fileWatcher;
    private readonly FileSystemWatcher _directoryWatcher;
    private readonly IMediaThumbnailService _mediaThumbnailService;
    private readonly IMediaPathService _mediaPathService;
    private readonly FileSystem _fileSystem;
    
    public MediaFileSystemWatcher(string path)
    {
        _fileWatcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.FileName,
            IncludeSubdirectories = true
        };
        _directoryWatcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.DirectoryName,
            IncludeSubdirectories = true
        };
    }
    
    public void Start()
    {
        _fileWatcher.EnableRaisingEvents = true;
        _directoryWatcher.EnableRaisingEvents = true;
        
        _fileWatcher.Deleted += OnFileChanged;
        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Renamed += OnFileRenamed;
        
        _directoryWatcher.Deleted += OnDirectoryChanged;
        _directoryWatcher.Changed += OnDirectoryChanged;
        _directoryWatcher.Renamed += OnDirectoryRenamed;
        
        
    }
    
    public void Stop()
    {
        _fileWatcher.EnableRaisingEvents = false;
        _directoryWatcher.EnableRaisingEvents = false;
        
        _fileWatcher.Deleted -= OnFileChanged;
        _fileWatcher.Changed -= OnFileChanged;
        _fileWatcher.Renamed -= OnFileRenamed;
        
        _directoryWatcher.Deleted -= OnDirectoryChanged;
        _directoryWatcher.Changed -= OnDirectoryChanged;
        _directoryWatcher.Renamed -= OnDirectoryRenamed;
    }

    private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
    {
        RelativePath relativePath = _mediaPathService.StripRootPath((AbsolutePath)e.FullPath);
       _mediaThumbnailService.DeleteThumbnailsForPath(relativePath);
    }
    
    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        RelativePath relativePath = _mediaPathService.StripRootPath((AbsolutePath)e.FullPath);
        _mediaThumbnailService.DeleteThumbnail(relativePath);
    }
    
    private void OnDirectoryRenamed(object sender, RenamedEventArgs e)
    {
        RelativePath oldRelativePath = _mediaPathService.StripRootPath((AbsolutePath)e.OldFullPath);
        _mediaThumbnailService.DeleteThumbnailsForPath(oldRelativePath);
    }
    
    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        RelativePath oldRelativePath = _mediaPathService.StripRootPath((AbsolutePath)e.OldFullPath);
        _mediaThumbnailService.DeleteThumbnail(oldRelativePath);
    }
}
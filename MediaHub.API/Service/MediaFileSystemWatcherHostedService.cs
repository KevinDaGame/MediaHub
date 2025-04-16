using System.IO.Abstractions;
using MediaHub.DAL.Model;
using MediaHub.DAL.Services;
using MediaHub.DAL.Services.MediaPath;

namespace MediaHub.API.Service;

public class MediaFileSystemWatcherHostedService : IHostedService
{
    private readonly FileSystemWatcher _fileWatcher;
    private readonly FileSystemWatcher _directoryWatcher;
    private readonly IMediaThumbnailService _mediaThumbnailService;
    private readonly RootPathService _mediaPathService;
    private readonly IFileSystem _fileSystem;

    public MediaFileSystemWatcherHostedService(
        RootPathService mediaPathService,
        IMediaThumbnailService mediaThumbnailService,
        IFileSystem fileSystem)
    {
        _mediaPathService = mediaPathService;
        _mediaThumbnailService = mediaThumbnailService;
        _fileSystem = fileSystem;

        _fileWatcher = new FileSystemWatcher(_mediaPathService.Path)
        {
            NotifyFilter = NotifyFilters.FileName,
            IncludeSubdirectories = true
        };
        _directoryWatcher = new FileSystemWatcher(_mediaPathService.Path)
        {
            NotifyFilter = NotifyFilters.DirectoryName,
            IncludeSubdirectories = true
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fileWatcher.EnableRaisingEvents = true;
        _directoryWatcher.EnableRaisingEvents = true;

        _fileWatcher.Deleted += OnFileChanged;
        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Renamed += OnFileRenamed;

        _directoryWatcher.Deleted += OnDirectoryChanged;
        _directoryWatcher.Changed += OnDirectoryChanged;
        _directoryWatcher.Renamed += OnDirectoryRenamed;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _fileWatcher.EnableRaisingEvents = false;
        _directoryWatcher.EnableRaisingEvents = false;

        _fileWatcher.Deleted -= OnFileChanged;
        _fileWatcher.Changed -= OnFileChanged;
        _fileWatcher.Renamed -= OnFileRenamed;

        _directoryWatcher.Deleted -= OnDirectoryChanged;
        _directoryWatcher.Changed -= OnDirectoryChanged;
        _directoryWatcher.Renamed -= OnDirectoryRenamed;

        return Task.CompletedTask;
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

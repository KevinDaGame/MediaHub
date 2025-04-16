using MediaHub.DAL.Services;

namespace MediaHub.API.Service;

public class ThumbnailHostedService : IHostedService
{
    private readonly IMediaThumbnailService _thumbnailService;
    private CancellationTokenSource _cts;
    private const int Interval = 1000 * 60;


    public ThumbnailHostedService(IMediaThumbnailService thumbnailService)
    {
        _thumbnailService = thumbnailService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Loop(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task Loop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Extracting thumbnails");
            _thumbnailService.ExtractThumbnailsForMediaFolder();
            await Task.Delay(Interval, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
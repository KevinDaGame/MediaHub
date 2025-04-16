using MediaHub.DAL.Services;

namespace MediaHub.API.Service;

public class MediaHostedService : IHostedService
{
    private CancellationTokenSource _cts;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private const int Interval = 1000 * 60;

    public MediaHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
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
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var mediaDiscoveryService = scope.ServiceProvider.GetRequiredService<IMediaDiscoveryService>();
                mediaDiscoveryService.DiscoverMedia();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(Interval, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mythral.Server.Domain.Game;
using Mythral.Server.Domain.Networking;

namespace Mythral.Server.Application.GameLoop;

public sealed class GameLoopService : BackgroundService
{
    private readonly IGameWorld _world;
    private readonly IUdpServer _udp;
    private readonly ITcpServer _tcp;
    private readonly ILogger<GameLoopService> _logger;

    // 60 Hz => ~16.666 ms
    private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(1000.0 / 60.0);

    public GameLoopService(IGameWorld world, IUdpServer udp, ITcpServer tcp, ILogger<GameLoopService> logger)
    {
        _world = world;
        _udp = udp;
        _tcp = tcp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("GameLoopService iniciado com intervalo {Interval} ms", TickInterval.TotalMilliseconds);
        var timer = new PeriodicTimer(TickInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var started = DateTime.UtcNow;
                // 1) drenar inputs (UDP/TCP)
                _udp.DrainInputs(_world.InputQueue);
                _tcp.DrainRequests(_world.RequestQueue);

                // 2) atualizar mundo
                _world.Update(TickInterval);

                // 3) broadcast snapshots
                _udp.BroadcastSnapshots(_world.VisibleStates);

                var elapsed = DateTime.UtcNow - started;
                if (elapsed > TickInterval)
                {
                    _logger.LogWarning("Tick excedeu limite: {Elapsed} ms (limite {Limit} ms)", elapsed.TotalMilliseconds, TickInterval.TotalMilliseconds);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected on shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha no loop principal do jogo");
            throw;
        }
        finally
        {
            _logger.LogInformation("GameLoopService finalizado");
        }
    }
}

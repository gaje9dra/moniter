namespace Moniter.Agent;

public sealed class AgentWorker(ILogger<AgentWorker> logger, AgentOptions options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Moniter agent foundation started. Version={Version}, Environment={EnvironmentName}",
            AgentVersion.Value,
            options.EnvironmentName);
        return Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Moniter agent foundation is shutting down.");
        await base.StopAsync(cancellationToken);
    }
}

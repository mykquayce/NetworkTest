using Dawn;
using Helpers.Timing;
using Microsoft.Extensions.Options;

namespace NetworkTest.WorkerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IInterval _interval;
	private readonly Services.IPacketLossTestService _packetLossTestService;
	private readonly Repositories.IRepository _repository;

	public Worker(
		ILogger<Worker> logger,
		IOptions<Interval> options,
		Services.IPacketLossTestService packetLossTestService,
		Repositories.IRepository repository)
	{
		_logger = logger;
		_interval = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
		_packetLossTestService = Guard.Argument(packetLossTestService).NotNull().Value;
		_repository = Guard.Argument(repository).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

			_logger.LogInformation("Pinging");
			var results = await _packetLossTestService.PacketLossTestAsync();
			var (_, count, _, loss, _, jitter) = results;
			_logger.LogInformation("Results: {count:D} ping(s), {loss:F2}% packet loss, {jitter:F2}ms jitter", count, loss, jitter);
			_logger.LogInformation("Saving.");
			await _repository.SaveResult(results);
			_logger.LogInformation("Saved.");

			var next = _interval.Next();
			_logger.LogInformation("Sleeping util {next:O}", next);
			var delay = next - DateTime.UtcNow;
			var millisecondInterval = (int)delay.TotalMilliseconds;
			await Task.Delay(millisecondInterval, stoppingToken);
		}
	}
}

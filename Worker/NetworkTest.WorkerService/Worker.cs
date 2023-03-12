using Dawn;
using Helpers.Timing;
using Microsoft.Extensions.Options;

namespace NetworkTest.WorkerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IInterval _interval;
	private readonly IServiceProvider _serviceProvider;

	public Worker(
		ILogger<Worker> logger,
		IOptions<Interval> options,
		IServiceProvider serviceProvider)
	{
		_logger = logger;
		_interval = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
		_serviceProvider = Guard.Argument(serviceProvider).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

			_logger.LogInformation("Pinging.");
			var results = await TestAsync();
			{
				var (_, count, _, loss, _, jitter) = results;
				_logger.LogInformation("Results: {count:D} ping(s), {loss:F2}% packet loss, {jitter:F2}ms jitter", count, loss, jitter);
			}

			await SaveAsync(results);

			DateTime next;
			{
				var min = DateTime.UtcNow.AddMinutes(2);
				next = _interval.GetUpcoming().First(dt => dt > min);
			}
			_logger.LogInformation("Sleeping util {next:O}", next);
			var delay = next - DateTime.UtcNow;
			var millisecondInterval = (int)delay.TotalMilliseconds;
			await Task.Delay(millisecondInterval, stoppingToken);
		}
	}

	private Task<Helpers.Networking.Models.PacketLossResults> TestAsync()
	{
		var service = _serviceProvider.GetRequiredService<Services.IPacketLossTestService>();
		return service.PacketLossTestAsync();
	}

	private async Task SaveAsync(Helpers.Networking.Models.PacketLossResults results)
	{
		var repository = _serviceProvider.GetRequiredService<Repositories.IRepository>();
		_logger.LogInformation("Saving.");
		await repository.SaveResult(results);
		_logger.LogInformation("Saved.");
	}
}

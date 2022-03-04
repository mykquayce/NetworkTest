using Dawn;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.WorkerService;

public class Worker : BackgroundService
{
	public record Config : Models.Interval, IOptions<Config>
	{
		public const Units DefaultUnit = Units.Day;
		public const double DefaultCount = 1;
		public Config() : base(DefaultUnit, DefaultCount) { }

		public Config Value => new();
	}

	private readonly ILogger<Worker> _logger;
	private readonly IWorkflowHost _workflowHost;
	private readonly Models.Interval _interval;

	public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost, IOptions<Config> options)
	{
		_logger = logger;
		_workflowHost = workflowHost;

		_interval = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Value;

		_workflowHost.OnStepError += WorkflowHost_OnStepError;
		_workflowHost.RegisterWorkflow<Workflows.MyWorkflow, Workflows.PersistenceData>();
		_workflowHost.Start();
	}

	private void WorkflowHost_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
	{
#if DEBUG
		System.Diagnostics.Debugger.Break();
#endif

		_logger.LogCritical(exception, "error processing step");
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

			var data = new Workflows.PersistenceData();

			var delay = _interval.Next - DateTime.UtcNow;
			var millisecondInterval = (int)delay.TotalMilliseconds;

			try
			{
				await Task.WhenAll(
					_workflowHost.StartWorkflow(nameof(Workflows.MyWorkflow), data),
					Task.Delay(millisecondInterval, stoppingToken));
			}
			catch (OperationCanceledException) { break; }
		}

		_workflowHost.Stop();
	}
}

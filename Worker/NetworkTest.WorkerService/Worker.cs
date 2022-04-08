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
	private readonly IPersistenceProvider _persistenceProvider;
	private readonly Models.Interval _interval;

	public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost, IPersistenceProvider persistenceProvider, IOptions<Config> options)
	{
		_logger = logger;
		_workflowHost = workflowHost;
		_persistenceProvider = persistenceProvider;

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

			{
				var data = new Workflows.PersistenceData();
				var id = await _workflowHost.StartWorkflow(nameof(Workflows.MyWorkflow), data);
				WorkflowStatus status;
				do
				{
					try { await Task.Delay(millisecondsDelay: 100, stoppingToken); }
					catch (OperationCanceledException) { break; }
					var instance = await _persistenceProvider.GetWorkflowInstance(id, stoppingToken);
					status = instance.Status;
				}
				while (status == WorkflowStatus.Runnable);
			}

			var delay = _interval.Next - DateTime.UtcNow;
			var millisecondInterval = (int)delay.TotalMilliseconds;
			await Task.Delay(millisecondInterval, stoppingToken);
		}

		_workflowHost.Stop();
	}
}

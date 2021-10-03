using Dawn;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.WorkerService;

public class Worker : BackgroundService
{
	public record Config(int MillisecondInterval)
	{
		public const int DefaultMillisecondInterval = 1_800_000;

		public Config() : this(DefaultMillisecondInterval) { }

		public static Config Default => new();
	}

	private readonly ILogger<Worker> _logger;
	private readonly IWorkflowHost _workflowHost;
	private readonly int _millisecondInterval;

	public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost, IOptions<Config> options)
	{
		_logger = logger;
		_workflowHost = workflowHost;

		_millisecondInterval = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().Wrap(c => c.MillisecondInterval)
			.Positive().Value;

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

			await _workflowHost.StartWorkflow(nameof(Workflows.MyWorkflow), data);

			try { await Task.Delay(_millisecondInterval, stoppingToken); }
			catch (OperationCanceledException) { break; }
		}

		_workflowHost.Stop();
	}
}

using Dawn;
using Microsoft.Extensions.Options;
using System.Net;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.WorkerService;
public class Worker : BackgroundService
{
	public record Config(int Interval, string Target)
	{
		public const int DefaultInterval = 1_800_000;
		public const string DefaultTarget = "lse.packetlosstest.com";

		public Config() : this(DefaultInterval, DefaultTarget) { }

		public static Config Default => new();
	}

	private readonly ILogger<Worker> _logger;
	private readonly IWorkflowHost _workflowHost;
	private readonly IPAddress _ip;
	private readonly int _interval;

	public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost, IOptions<Config> options)
	{
		_logger = logger;
		_workflowHost = workflowHost;

		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;

		var target = Guard.Argument(config).Wrap(c => c.Target).NotNull().NotEmpty().NotWhiteSpace();
		_ip = Dns.GetHostAddresses(target).FirstOrDefault() ?? throw new Exception();
		_interval = Guard.Argument(config).Wrap(c => c.Interval).Positive().Value;

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

				var data = new Workflows.PersistenceData { IPAddress = _ip, };

				await _workflowHost.StartWorkflow(nameof(Workflows.MyWorkflow), data);

			try { await Task.Delay(_interval, stoppingToken); }
			catch (OperationCanceledException) { return; }
		}

		_workflowHost.Stop();
	}
}

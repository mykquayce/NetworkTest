using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.Workflows.Steps;

public class PingStep : IStepBody
{
	private readonly ILogger<PingStep> _logger;
	private readonly Services.IPacketLossTestService _pingService;

	public PingStep(ILogger<PingStep> logger, Services.IPacketLossTestService pingService)
	{
		_logger = Guard.Argument(logger).NotNull().Value;
		_pingService = Guard.Argument(pingService).NotNull().Value;
	}

	public Helpers.Networking.Models.PacketLossResults? Results { get; set; }

	public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
	{
		_logger.LogInformation("Pinging");
		Results = await _pingService.PacketLossTestAsync();
		var (_, count, _, loss, _, jitter) = Results;
		_logger.LogInformation("Results: {count:D} ping(s), {loss:F2}% packet loss, {jitter:F2}ms jitter", count, loss, jitter);
		return ExecutionResult.Next();
	}
}

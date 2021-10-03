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
		_logger.LogDebug("Pinging");
		Results = await _pingService.PacketLossTestAsync();
		_logger.LogDebug($"Results: {Results.Count} ping(s), {Results.PacketLossPercentage:F2}% packet loss");
		return ExecutionResult.Next();
	}
}

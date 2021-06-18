using Dawn;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.Workflows.Steps
{
	public class PingStep : IStepBody
	{
		private readonly ILogger<PingStep> _logger;
		private readonly Helpers.Networking.Clients.IPingClient _client;

		public PingStep(ILogger<PingStep> logger, Helpers.Networking.Clients.IPingClient client)
		{
			_logger = Guard.Argument(() => logger).NotNull().Value;
			_client = Guard.Argument(() => client).NotNull().Value;
		}

		public IPAddress? IPAddress { get; set; }
		public Helpers.Networking.Models.PacketLossResults? Results { get; set; }

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => IPAddress!).NotNull();
			_logger.LogDebug($"Pinging: {IPAddress}");
			Results = await _client.PacketLossTestAsync(IPAddress!);
			_logger.LogDebug($"Results: {Results.Count} ping(s), {Results.PacketLossPercentage:F2}% packet loss");
			return ExecutionResult.Next();
		}
	}
}

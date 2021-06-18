using Dawn;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace NetworkTest.Workflows.Steps
{
	public class SaveStep : IStepBody
	{
		private readonly ILogger<SaveStep> _logger;
		private readonly Repositories.IRepository _repository;

		public SaveStep(ILogger<SaveStep> logger, Repositories.IRepository repository)
		{
			_logger = Guard.Argument(() => logger).NotNull().Value;
			_repository = Guard.Argument(() => repository).NotNull().Value;
		}

		public Helpers.Networking.Models.PacketLossResults? Results { get; set; }

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => Results!).NotNull();
			_logger.LogDebug($"Saving: {Results!.Count} ping(s), {Results.PacketLossPercentage:F2}% packet loss");
			await _repository.SaveResult(Results!);
			_logger.LogDebug("Saved.");
			return ExecutionResult.Next();
		}
	}
}

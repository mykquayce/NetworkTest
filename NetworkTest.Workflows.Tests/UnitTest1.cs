using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Xunit;

namespace NetworkTest.Workflows.Tests;

public class UnitTest1
{
	[Fact]
	public async Task Test1()
	{
		IWorkflowHost workflowHost;
		{
			var configCollection = new Dictionary<string, string>
			{
				["Ping:Timeout"] = "2000",
				["Test:HostNameOrIPAddress"] = "lse.packetlosstest.com",
				["Test:MillisecondInterval"] = "1800000",
				["Test:MillisecondDuration"] = "20000",
				["Database:Server"] = "localhost",
				["Database:Database"] = "networktest",
				["Database:Port"] = "3306",
				["Database:UserId"] = "networktest",
				["Database:Password"] = "6MgYJFucyfBDXZeTE2Knu2wIJZQudMwWo0U560caAlVSdMoycXTqSgsZaoWFYKFY0CZJwoQ0IwUrVKVxRj6CyUKTCClKjNqkr7MzAX8gSmo7Cpdhr0OenCNTb5yMiGaU4rdA7JMaXwDS7fHdOREYBUe64ZYnAd7CuE4CfSUPXwWKJKJC01WEzIFdt49UGsVQnOIk7jgq4es7MfP5M3SXCB4zC7JrsVdjMxaCbl7zzwwQPk0xPqQroDx6QTWUf1ZAyi",
			};

			var services = new ServiceCollection();
			services.AddLogging();
			services.AddWorkflow();
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(configCollection)
				.Build();
			services
				.Configure<NetworkTest.Services.Concrete.PacketLossTestService.Config>(configuration.GetSection("Ping"))
				.Configure<Helpers.Networking.Clients.Concrete.PingClient.Config>(configuration.GetSection("Ping"))
				.Configure<NetworkTest.Repositories.Concrete.Repository.Config>(configuration.GetSection("Database"));

			services
				.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>()
				.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();

			services
				.AddTransient<Steps.PingStep>()
				.AddTransient<Steps.SaveStep>();

			var serviceProvider = services.BuildServiceProvider();
			workflowHost = serviceProvider.GetRequiredService<IWorkflowHost>();
		}

		workflowHost.RegisterWorkflow<MyWorkflow, PersistenceData>();
		workflowHost.Start();

		workflowHost.OnStepError += WorkflowHost_OnStepError;

		var data = new PersistenceData();

		var workflowInstanceId = await workflowHost.StartWorkflow(nameof(MyWorkflow), data);

		await Task.Delay(millisecondsDelay: 20_000);

		workflowHost.Stop();

		var workflowInstance = await workflowHost.PersistenceStore.GetWorkflowInstance(workflowInstanceId);

		var data2 = (PersistenceData)workflowInstance.Data;

		Assert.NotNull(data2.Results);
	}

	private void WorkflowHost_OnStepError(WorkflowInstance workflow, WorkflowStep step, System.Exception exception)
	{
		Assert.True(false, exception.Message);
	}
}

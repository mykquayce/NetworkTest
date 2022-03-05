using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
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
				["Test:HostNameOrIPAddress"] = "lau.packetlosstest.com",
				["Test:MillisecondDuration"] = "15000",
				["Database:Server"] = "localhost",
				["Database:Database"] = "networktest",
				["Database:Port"] = "3306",
				["Database:UserId"] = "networktest",
				["Database:Password"] = "networktest",
			};

			var services = new ServiceCollection();
			services.AddLogging();
			services.AddWorkflow();
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(configCollection)
				.Build();
			services
				.Configure<NetworkTest.Services.Concrete.PacketLossTestService.Config>(configuration.GetSection("Test"))
				.Configure<Helpers.Networking.Clients.Concrete.PingClient.Config>(configuration.GetSection("Ping"))
				.Configure<Helpers.MySql.Config>(configuration.GetSection("Database"));

			services
				.AddTransient<Services.IPacketLossTestService, Services.Concrete.PacketLossTestService>()
				.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>();

			services
				.AddTransient<System.Data.IDbConnection>(provider =>
				{
					var options = provider.GetRequiredService<IOptions<Helpers.MySql.Config>>();
					return new MySql.Data.MySqlClient.MySqlConnection(options.Value.ConnectionString);
				})
				.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();

			services
				.AddTransient<Steps.PingStep>()
				.AddTransient<Steps.SaveStep>();

			var serviceProvider = services.BuildServiceProvider();
			workflowHost = serviceProvider.GetRequiredService<IWorkflowHost>();
		}

		workflowHost.RegisterWorkflow<MyWorkflow, PersistenceData>();
		workflowHost.Start();

		workflowHost.OnStepError += (_, step, exception) => Assert.True(false, step.Name + ";" + exception.Message);

		var data = new PersistenceData();
		Assert.Null(data.Results);

		var workflowInstanceId = await workflowHost.StartWorkflow(nameof(MyWorkflow), data);

		await Task.Delay(millisecondsDelay: 20_000);

		workflowHost.Stop();

		Assert.NotNull(data.Results);
		var now = DateTime.UtcNow;
		Assert.InRange(data.Results!.DateTime, now.AddMinutes(-1), now);
		Assert.InRange(data.Results.Count, 10, 100);
	}
}

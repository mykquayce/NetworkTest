using Microsoft.Extensions.Options;
using NetworkTest.WorkerService;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHostedService<Worker>();

		services.AddLogging();
		services.AddWorkflow(options =>
		{
			options.UsePollInterval(TimeSpan.FromMinutes(1));
		});

		services
			.Configure<Helpers.Networking.Clients.Concrete.PingClient.Config>(hostContext.Configuration.GetSection("Ping"))
			.Configure<NetworkTest.Services.Concrete.PacketLossTestService.Config>(hostContext.Configuration.GetSection("Test"))
			.FileConfigure<Helpers.MySql.Config>(hostContext.Configuration.GetSection("Database"))
			.Configure<NetworkTest.WorkerService.Worker.Config>(hostContext.Configuration.GetSection("Test").GetSection("Interval"));

		services
			.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>()
			.AddTransient<NetworkTest.Services.IPacketLossTestService, NetworkTest.Services.Concrete.PacketLossTestService>()
			.AddTransient<System.Data.IDbConnection>(provider =>
			{
				var options = provider.GetRequiredService<IOptions<Helpers.MySql.Config>>();
				return new MySql.Data.MySqlClient.MySqlConnection(options.Value.ConnectionString);
			})
			.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();

		services
			.AddTransient<NetworkTest.Workflows.Steps.PingStep>()
			.AddTransient<NetworkTest.Workflows.Steps.SaveStep>();
	});

var host = hostBuilder
	.Build();

await host.RunAsync();

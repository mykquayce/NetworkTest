using Microsoft.Extensions.Options;
using NetworkTest.WorkerService;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder
	.ConfigureAppConfiguration(builder =>
	{
		builder
			.ResolveFileReferences();
	})
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHostedService<Worker>();

		services.AddLogging();

		services
			.Configure<Helpers.Networking.Clients.Concrete.PingClient.Config>(hostContext.Configuration.GetSection("Ping"))
			.Configure<NetworkTest.Services.Concrete.PacketLossTestService.Config>(hostContext.Configuration.GetSection("Test"))
			.Configure<Helpers.MySql.Config>(hostContext.Configuration.GetSection("Database"))
			.Configure<Helpers.Timing.Interval>(hostContext.Configuration.GetSection("Test").GetSection("Interval"));

		services
			.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>()
			.AddTransient<NetworkTest.Services.IPacketLossTestService, NetworkTest.Services.Concrete.PacketLossTestService>()
			.AddTransient<System.Data.IDbConnection>(provider =>
			{
				var options = provider.GetRequiredService<IOptions<Helpers.MySql.Config>>();
				return new MySqlConnector.MySqlConnection(options.Value.ConnectionString);
			})
			.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();
	});

var host = hostBuilder
	.Build();

await host.RunAsync();

using NetworkTest.WorkerService;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder
	.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
	{
		configurationBuilder
			.AddDockerSecrets(optional: true, reloadOnChange: true, filenameCharsToSwapWithColons: '_');
	});

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
			.Configure<NetworkTest.Repositories.Concrete.Repository.Config>(hostContext.Configuration.GetSection("Database"))
			.Configure<NetworkTest.WorkerService.Worker.Config>(hostContext.Configuration);

		services
			.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>()
			.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();

		services
			.AddTransient<NetworkTest.Workflows.Steps.PingStep>()
			.AddTransient<NetworkTest.Workflows.Steps.SaveStep>();
	});

var host = hostBuilder
	.Build();

await host.RunAsync();

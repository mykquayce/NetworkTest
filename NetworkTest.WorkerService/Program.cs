using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace NetworkTest.WorkerService
{
	public class Program
	{
		public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
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
					services.AddWorkflow();

					services
						.Configure<Helpers.Networking.Clients.Concrete.PingClient.Config>(hostContext.Configuration.GetSection("Ping"))
						.Configure<NetworkTest.Repositories.Concrete.Repository.Config>(hostContext.Configuration.GetSection("Database"))
						.Configure<Worker.Config>(hostContext.Configuration);

					services
						.AddTransient<Helpers.Networking.Clients.IPingClient, Helpers.Networking.Clients.Concrete.PingClient>()
						.AddTransient<NetworkTest.Repositories.IRepository, NetworkTest.Repositories.Concrete.Repository>();

					services
						.AddTransient<Workflows.Steps.PingStep>()
						.AddTransient<Workflows.Steps.SaveStep>();
				});

			return hostBuilder;
		}
	}
}

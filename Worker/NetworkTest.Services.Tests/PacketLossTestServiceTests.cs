using Xunit;

namespace NetworkTest.Services.Tests;

public class PacketLossTestServiceTests
{
	private readonly Helpers.Networking.Clients.IPingClient _client;

	public PacketLossTestServiceTests()
	{
		var pingClientConfig = Helpers.Networking.Clients.Concrete.PingClient.Config.Defaults;
		_client = new Helpers.Networking.Clients.Concrete.PingClient(pingClientConfig);
	}

	[Theory]
	[InlineData("172.105.174.146", 20_000)]
	[InlineData("172.105.174.146", 30_000)]
	public async Task Durations(string hostNameOrIPAddress, int milliseconds)
	{
		// Arrange
		var config = new Concrete.PacketLossTestService.Config(hostNameOrIPAddress, milliseconds);
		var sut = new Concrete.PacketLossTestService(_client, config);

		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		// Act
		var results = await sut.PacketLossTestAsync();

		// Arrange
		stopwatch.Stop();

		// Assert
		Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, milliseconds, milliseconds + 1_000);
		Assert.InRange(results.Count, 1, int.MaxValue);
	}
}

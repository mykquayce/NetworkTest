using Dawn;
using Microsoft.Extensions.Options;
using System.Net;

namespace NetworkTest.Services.Concrete;

public class PacketLossTestService : IPacketLossTestService
{
	#region config
	public record Config(string HostNameOrIPAddress, int MillisecondDuration)
		: IOptions<Config>
	{
		public const string DefaultHostNameOrIPAddress = "lse.packetlosstest.com";
		public const int DefaultMillisecondDuration = 10_000;

		public Config() : this(DefaultHostNameOrIPAddress, DefaultMillisecondDuration) { }

		public static Config Defaults => new();

		#region ioptions implementation
		public Config Value => this;
		#endregion ioptions implementation
	}
	#endregion config

	private readonly Helpers.Networking.Clients.IPingClient _client;
	private readonly int _testDuration;
	private readonly IPAddress _ipAddress;

	public PacketLossTestService(Helpers.Networking.Clients.IPingClient client, IOptions<Config> options)
	{
		_client = Guard.Argument(client).NotNull().Value;

		var config = Guard.Argument(options).NotNull().Value.Value;

		var hostNameOrIPAddress = Guard.Argument(config).Wrap(c => c.HostNameOrIPAddress)
			.NotNull().NotEmpty().NotWhiteSpace().Value;

		_ipAddress = GetIPAddress(hostNameOrIPAddress);
		_testDuration = Guard.Argument(config).Wrap(c => c.MillisecondDuration)
			.Positive().Value;
	}

	public Task<Helpers.Networking.Models.PacketLossResults> PacketLossTestAsync()
		=> _client.PacketLossTestAsync(_ipAddress, _testDuration);

	private static IPAddress GetIPAddress(string hostNameOrIPAddress)
	{
		Guard.Argument(hostNameOrIPAddress).NotNull().NotEmpty().NotWhiteSpace();

		return Dns.GetHostAddresses(hostNameOrIPAddress).FirstOrDefault()
			?? throw new ArgumentOutOfRangeException(nameof(hostNameOrIPAddress), hostNameOrIPAddress, "could not get host address from " + hostNameOrIPAddress)
			{
				Data = { [nameof(hostNameOrIPAddress)] = hostNameOrIPAddress, },
			};
	}
}

using Helpers.Networking.Models;

namespace NetworkTest.Services
{
	public interface IPacketLossTestService
	{
		Task<PacketLossResults> PacketLossTestAsync();
	}
}

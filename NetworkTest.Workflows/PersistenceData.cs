using System.Net;

namespace NetworkTest.Workflows
{
	public class PersistenceData
	{
		public IPAddress? IPAddress { get; set; }
		public Helpers.Networking.Models.PacketLossResults? Results { get; set; }
	}
}

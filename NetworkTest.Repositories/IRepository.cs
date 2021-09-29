namespace NetworkTest.Repositories;

public interface IRepository : IDisposable
{
	Task DeleteResult(DateTime dateTime);
	Task<Helpers.Networking.Models.PacketLossResults> GetResult(DateTime dateTime);
	Task SaveResult(Helpers.Networking.Models.PacketLossResults result);
	Task UpdateResult(Helpers.Networking.Models.PacketLossResults result);
}

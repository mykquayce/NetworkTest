namespace NetworkTest.Repositories;

public interface IRepository
{
	Task DeleteResult(DateTime dateTime);
	ValueTask<Helpers.Networking.Models.PacketLossResults?> GetResult(DateTime dateTime);
	Task SaveResult(Helpers.Networking.Models.PacketLossResults result);
	Task UpdateResult(Helpers.Networking.Models.PacketLossResults result);
}

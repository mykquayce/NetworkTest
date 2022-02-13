using Dapper;
using Dawn;
using System.Data;
using System.Linq;

namespace NetworkTest.Repositories.Concrete;

public class Repository : Helpers.MySql.RepositoryBase, IRepository
{
	public Repository(IDbConnection connection)
		: base(connection)
	{ }

	public Task DeleteResult(DateTime dateTime)
	{
		Guard.Argument(dateTime).NotDefault().LessThan(DateTime.UtcNow);
		return base.ExecuteAsync("DELETE FROM Results WHERE DateTime = @dateTime;", param: new { dateTime, });
	}

	public ValueTask<Helpers.Networking.Models.PacketLossResults?> GetResult(DateTime dateTime)
	{
		Guard.Argument(dateTime).NotDefault().LessThan(DateTime.UtcNow);
		return base.QueryAsync<Helpers.Networking.Models.PacketLossResults>("SELECT * FROM Results WHERE DateTime = @dateTime LIMIT 1;", param: new { dateTime, })
			.FirstOrDefaultAsync();
	}

	public Task SaveResult(Helpers.Networking.Models.PacketLossResults result)
	{
		Guard.Argument(result).NotNull();
		return base.ExecuteAsync(
			"INSERT Results (DateTime, Count, FailedCount, PacketLossPercentage, AverageRoundtripTime, AverageJitter) VALUES (@DateTime, @Count, @FailedCount, @PacketLossPercentage, @AverageRoundtripTime, @AverageJitter);",
			param: result);
	}

	public Task UpdateResult(Helpers.Networking.Models.PacketLossResults result)
	{
		Guard.Argument(result).NotNull();
		return base.ExecuteAsync(@"UPDATE Results
				SET Count = @Count,
					FailedCount = @FailedCount,
					PacketLossPercentage = @PacketLossPercentage,
					AverageRoundtripTime = @AverageRoundtripTime,
					AverageJitter = @AverageJitter
				WHERE DateTime = @dateTime;", param: result);
	}
}

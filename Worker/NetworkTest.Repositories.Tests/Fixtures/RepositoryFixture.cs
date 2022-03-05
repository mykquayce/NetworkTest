using System.Data;

namespace NetworkTest.Repositories.Tests.Fixtures;

public sealed class RepositoryFixture : IDisposable
{
	private readonly IDbConnection _connection;

	public RepositoryFixture()
	{
		var config = new Helpers.MySql.Config("localhost", 3_306, "networktest", "networktest", "networktest", Secure: true);

		_connection = config.DbConnection;

		Repository = new Concrete.Repository(_connection);
	}

	public IRepository Repository { get; }

	public void Dispose() => _connection.Dispose();
}

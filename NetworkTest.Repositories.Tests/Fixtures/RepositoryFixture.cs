using System.Data;

namespace NetworkTest.Repositories.Tests.Fixtures;

public sealed class RepositoryFixture : IDisposable
{
	private readonly IDbConnection _connection;

	public RepositoryFixture()
	{
		var userSecretsFixture = new UserSecretsFixture();

		var server = userSecretsFixture["Database:Server"];
		var port = uint.Parse(userSecretsFixture["Database:Port"]);
		var database = userSecretsFixture["Database:Database"];
		var secure = bool.Parse(userSecretsFixture["Database:Secure"]);
		var userId = userSecretsFixture["Database:UserId"];
		var password = userSecretsFixture["Database:Password"];

		var config = new Helpers.MySql.Config(server, port, database, userId, password, secure);

		_connection = config.DbConnection;

		Repository = new Concrete.Repository(_connection);
	}

	public IRepository Repository { get; }

	public void Dispose() => _connection.Dispose();
}

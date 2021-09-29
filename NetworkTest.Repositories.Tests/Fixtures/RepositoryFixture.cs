using Microsoft.Extensions.Options;

namespace NetworkTest.Repositories.Tests.Fixtures;

public class RepositoryFixture : IDisposable
{
	public RepositoryFixture()
	{
		var userSecretsFixture = new UserSecretsFixture();

		var server = userSecretsFixture["Database:Server"];
		var database = userSecretsFixture["Database:Database"];
		var port = uint.Parse(userSecretsFixture["Database:Port"]);
		var sslMode = Enum.Parse<MySql.Data.MySqlClient.MySqlSslMode>(userSecretsFixture["Database:Port"]);
		var userId = userSecretsFixture["Database:UserId"];
		var password = userSecretsFixture["Database:Password"];


		var config = new Concrete.Repository.Config(server, database, port, sslMode, userId, password);

		var options = Options.Create(config);

		Repository = new Concrete.Repository(options);
	}

	public IRepository Repository { get; }

	#region disposing
	private bool _disposing;

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				Repository.Dispose();
			}

			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion disposing
}

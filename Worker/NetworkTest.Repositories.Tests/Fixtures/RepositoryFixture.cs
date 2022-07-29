using Microsoft.Extensions.Configuration;
using System.Data;

namespace NetworkTest.Repositories.Tests.Fixtures;

public sealed class RepositoryFixture : IDisposable
{
	private readonly IDbConnection _connection;

	public RepositoryFixture()
	{
		Helpers.MySql.Config config;
		{
			config = Helpers.MySql.Config.Defaults;

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();

			configuration.GetSection("Database").Bind(config);
		}

		_connection = config.DbConnection;

		Repository = new Concrete.Repository(_connection);
	}

	public IRepository Repository { get; }

	public void Dispose() => _connection.Dispose();
}

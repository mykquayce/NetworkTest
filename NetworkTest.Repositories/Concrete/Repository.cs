using Dapper;
using Dawn;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace NetworkTest.Repositories.Concrete
{
	public class Repository : IRepository
	{
		public record Config(string Server, string Database, uint Port, MySqlSslMode SslMode, string UserId, string Password)
		{
			public const string DefaultServer = "localhost";
			public const string DefaultDatabase = "db";
			public const uint DefaultPort = 3_306;
			public const MySqlSslMode DefaultSslMode = MySqlSslMode.None;
			public const string DefaultUserId = "root";
			public const string DefaultPassword = "password";

			public Config() : this(DefaultServer, DefaultDatabase, DefaultPort, DefaultSslMode, DefaultUserId, DefaultPassword) { }

			public static Config Defaults => new();
		}

		private readonly IDbConnection _connection;

		#region constructor
		public Repository(IOptions<Config> options)
		{
			var config = Guard.Argument(() => options).NotNull().Wrap(o => o.Value).NotNull().Value;

			var builder = new MySqlConnectionStringBuilder
			{
				Server = Guard.Argument(() => config.Server).NotNull().NotEmpty().NotWhiteSpace().Value,
				Database = Guard.Argument(() => config.Database).NotNull().NotEmpty().NotWhiteSpace().Value,
				Port = Guard.Argument(() => config.Port).Positive().Value,
				SslMode = Guard.Argument(() => config.SslMode).Defined().Value,
				UserID = Guard.Argument(() => config.UserId).NotNull().NotEmpty().NotWhiteSpace().Value,
				Password = Guard.Argument(() => config.Password).NotNull().NotEmpty().NotWhiteSpace().Value,
			};

			var connectionString = builder.ConnectionString;

			_connection = new MySqlConnection(connectionString);
		}
		#endregion constructor

		public Task DeleteResult(DateTime dateTime)
		{
			Guard.Argument(() => dateTime).NotDefault().LessThan(DateTime.UtcNow);
			return _connection.ExecuteAsync("DELETE FROM Results WHERE DateTime = @dateTime;", param: new { dateTime, });
		}

		public Task<Helpers.Networking.Models.PacketLossResults> GetResult(DateTime dateTime)
		{
			Guard.Argument(() => dateTime).NotDefault().LessThan(DateTime.UtcNow);
			return _connection.QueryFirstOrDefaultAsync<Helpers.Networking.Models.PacketLossResults>("SELECT * FROM Results WHERE DateTime = @dateTime LIMIT 1;", param: new { dateTime, });
		}

		public Task SaveResult(Helpers.Networking.Models.PacketLossResults result)
		{
			Guard.Argument(() => result).NotNull();
			return _connection.ExecuteAsync(
				"INSERT Results (DateTime, Count, FailedCount, PacketLossPercentage, AverageRoundtripTime, AverageJitter) VALUES (@DateTime, @Count, @FailedCount, @PacketLossPercentage, @AverageRoundtripTime, @AverageJitter);",
				param: result);
		}

		public Task UpdateResult(Helpers.Networking.Models.PacketLossResults result)
		{
			Guard.Argument(() => result).NotNull();
			return _connection.ExecuteAsync(@"UPDATE Results
				SET Count = @Count,
					FailedCount = @FailedCount,
					PacketLossPercentage = @PacketLossPercentage,
					AverageRoundtripTime = @AverageRoundtripTime,
					AverageJitter = @AverageJitter
				WHERE DateTime = @dateTime;", param: result);
		}

		#region disposing
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_connection.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion disposing
	}
}

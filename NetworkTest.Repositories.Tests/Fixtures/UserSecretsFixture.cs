using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace NetworkTest.Repositories.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private readonly IConfiguration _configuration;

		public UserSecretsFixture()
		{
			_configuration = new ConfigurationBuilder()
				.AddUserSecrets<UserSecretsFixture>()
				.Build();
		}

		public string this[string key] => _configuration[key]
			?? throw new KeyNotFoundException($"{key} {nameof(key)} not found");
	}
}

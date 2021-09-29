using System.Globalization;
using Xunit;

namespace NetworkTest.Repositories.Tests
{
	public class DatabaseTests : IClassFixture<Fixtures.RepositoryFixture>
	{
		private readonly IRepository _repository;

		public DatabaseTests(Fixtures.RepositoryFixture fixture)
		{
			_repository = fixture.Repository;
		}

		[Theory]
		[InlineData("2021-06-16T02:56:14Z", 131, 11, 8.4f, 120.11f, 18.69f)]
		public async Task InsertSelectUpdateDeleteTests(string dateTimeString, int count, int failedCount, float packetLossPercentage, float averageRoundtripTime, float averageJitter)
		{
			// Arrange
			var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
			var one = new Helpers.Networking.Models.PacketLossResults(dateTime, count, failedCount, packetLossPercentage, averageRoundtripTime, averageJitter);

			// Act: Insert
			await _repository.SaveResult(one);

			// Act: Select
			var two = await _repository.GetResult(dateTime);

			Assert.False(ReferenceEquals(one, two));
			Assert.Equal(one, two);

			// Assert
			Assert.NotNull(two);
			Assert.Equal(dateTime, two.DateTime);
			Assert.Equal(count, two.Count);
			Assert.Equal(failedCount, two.FailedCount);
			Assert.Equal(packetLossPercentage, two.PacketLossPercentage);
			Assert.Equal(averageRoundtripTime, two.AverageRoundtripTime);
			Assert.Equal(averageJitter, two.AverageJitter);

			// Act: Update
			await _repository.UpdateResult(one with { Count = -one.Count, });

			// Act: Select #2
			two = await _repository.GetResult(dateTime);

			// Assert
			Assert.NotNull(two);
			Assert.Equal(-count, two.Count);

			// Act: Delete
			await _repository.DeleteResult(dateTime);

			// Act: Select #3
			two = await _repository.GetResult(dateTime);

			Assert.Null(two);
		}
	}
}

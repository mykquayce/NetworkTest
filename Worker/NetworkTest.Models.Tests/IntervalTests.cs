using Xunit;

namespace NetworkTest.Models.Tests;

public class IntervalTests
{
	public IntervalTests()
	{
		Interval.GetUtcNow = () => DateTime.Parse("2022-03-02T17:10:20.0431470Z");
	}

	[Theory]
	[InlineData(Interval.Units.Day, .25, "2022-03-02T12:00:00.0000000Z", "2022-03-02T06:00:00.0000000Z", "2022-03-02T00:00:00.0000000Z", "2022-03-01T18:00:00.0000000Z", "2022-03-01T12:00:00.0000000Z")]
	[InlineData(Interval.Units.Day, 1, "2022-03-02T00:00:00.0000000Z", "2022-03-01T00:00:00.0000000Z", "2022-02-28T00:00:00.0000000Z", "2022-02-27T00:00:00.0000000Z", "2022-02-26T00:00:00.0000000Z")]
	[InlineData(Interval.Units.Day, 4, "2022-02-27T00:00:00.0000000Z", "2022-02-23T00:00:00.0000000Z", "2022-02-19T00:00:00.0000000Z", "2022-02-15T00:00:00.0000000Z", "2022-02-11T00:00:00.0000000Z")]
	[InlineData(Interval.Units.Hour, .5, "2022-03-02T17:00:00.0000000Z", "2022-03-02T16:30:00.0000000Z", "2022-03-02T16:00:00.0000000Z", "2022-03-02T15:30:00.0000000Z", "2022-03-02T15:00:00.0000000Z")]
	[InlineData(Interval.Units.Hour, 1, "2022-03-02T17:00:00.0000000Z", "2022-03-02T16:00:00.0000000Z", "2022-03-02T15:00:00.0000000Z", "2022-03-02T14:00:00.0000000Z", "2022-03-02T13:00:00.0000000Z")]
	[InlineData(Interval.Units.Hour, 2, "2022-03-02T16:00:00.0000000Z", "2022-03-02T14:00:00.0000000Z", "2022-03-02T12:00:00.0000000Z", "2022-03-02T10:00:00.0000000Z", "2022-03-02T08:00:00.0000000Z")]
	[InlineData(Interval.Units.Minute, .5, "2022-03-02T17:10:00.0000000Z", "2022-03-02T17:09:30.0000000Z", "2022-03-02T17:09:00.0000000Z", "2022-03-02T17:08:30.0000000Z", "2022-03-02T17:08:00.0000000Z")]
	[InlineData(Interval.Units.Minute, 1, "2022-03-02T17:10:00.0000000Z", "2022-03-02T17:09:00.0000000Z", "2022-03-02T17:08:00.0000000Z", "2022-03-02T17:07:00.0000000Z", "2022-03-02T17:06:00.0000000Z")]
	[InlineData(Interval.Units.Minute, 2, "2022-03-02T17:10:00.0000000Z", "2022-03-02T17:08:00.0000000Z", "2022-03-02T17:06:00.0000000Z", "2022-03-02T17:04:00.0000000Z", "2022-03-02T17:02:00.0000000Z")]
	[InlineData(Interval.Units.Second, 1, "2022-03-02T17:10:20.0000000Z", "2022-03-02T17:10:19.0000000Z", "2022-03-02T17:10:18.0000000Z", "2022-03-02T17:10:17.0000000Z", "2022-03-02T17:10:16.0000000Z")]
	public void GetExpired(Interval.Units unit, double count, params string[] expecteds)
	{
		// Arrange
		var interval = new Interval(unit, count);

		// Act
		var actuals = interval.GetExpired()
			.Take(expecteds.Length)
			.ToList();

		// Assert
		for (var a = 0; a < actuals.Count; a++)
		{
			var expected = expecteds[a];
			var actual = actuals[a];
			Assert.Equal(expected, actual.ToString("O"));
		}
	}

	[Theory]
	[InlineData(Interval.Units.Day, .25, "2022-03-02T18:00:00.0000000Z", "2022-03-03T00:00:00.0000000Z", "2022-03-03T06:00:00.0000000Z", "2022-03-03T12:00:00.0000000Z", "2022-03-03T18:00:00.0000000Z")]
	[InlineData(Interval.Units.Day, 1, "2022-03-03T00:00:00.0000000Z", "2022-03-04T00:00:00.0000000Z", "2022-03-05T00:00:00.0000000Z", "2022-03-06T00:00:00.0000000Z", "2022-03-07T00:00:00.0000000Z")]
	[InlineData(Interval.Units.Day, 4, "2022-03-03T00:00:00.0000000Z", "2022-03-07T00:00:00.0000000Z", "2022-03-11T00:00:00.0000000Z", "2022-03-15T00:00:00.0000000Z", "2022-03-19T00:00:00.0000000Z")]
	[InlineData(Interval.Units.Hour, .5, "2022-03-02T17:30:00.0000000Z", "2022-03-02T18:00:00.0000000Z", "2022-03-02T18:30:00.0000000Z", "2022-03-02T19:00:00.0000000Z", "2022-03-02T19:30:00.0000000Z")]
	[InlineData(Interval.Units.Hour, 1, "2022-03-02T18:00:00.0000000Z", "2022-03-02T19:00:00.0000000Z", "2022-03-02T20:00:00.0000000Z", "2022-03-02T21:00:00.0000000Z", "2022-03-02T22:00:00.0000000Z")]
	[InlineData(Interval.Units.Hour, 2, "2022-03-02T18:00:00.0000000Z", "2022-03-02T20:00:00.0000000Z", "2022-03-02T22:00:00.0000000Z", "2022-03-03T00:00:00.0000000Z", "2022-03-03T02:00:00.0000000Z")]
	[InlineData(Interval.Units.Minute, .5, "2022-03-02T17:10:30.0000000Z", "2022-03-02T17:11:00.0000000Z", "2022-03-02T17:11:30.0000000Z", "2022-03-02T17:12:00.0000000Z", "2022-03-02T17:12:30.0000000Z")]
	[InlineData(Interval.Units.Minute, 1, "2022-03-02T17:11:00.0000000Z", "2022-03-02T17:12:00.0000000Z", "2022-03-02T17:13:00.0000000Z", "2022-03-02T17:14:00.0000000Z", "2022-03-02T17:15:00.0000000Z")]
	[InlineData(Interval.Units.Minute, 2, "2022-03-02T17:12:00.0000000Z", "2022-03-02T17:14:00.0000000Z", "2022-03-02T17:16:00.0000000Z", "2022-03-02T17:18:00.0000000Z", "2022-03-02T17:20:00.0000000Z")]
	[InlineData(Interval.Units.Second, 1, "2022-03-02T17:10:21.0000000Z", "2022-03-02T17:10:22.0000000Z", "2022-03-02T17:10:23.0000000Z", "2022-03-02T17:10:24.0000000Z", "2022-03-02T17:10:25.0000000Z")]
	public void GetUpcoming(Interval.Units unit, double count, params string[] expecteds)
	{
		// Arrange
		var interval = new Interval(unit, count);

		// Act
		var actuals = interval.GetUpcoming()
			.Take(expecteds.Length)
			.ToList();

		// Assert
		for (var a = 0; a < actuals.Count; a++)
		{
			var expected = expecteds[a];
			var actual = actuals[a];
			Assert.Equal(expected, actual.ToString("O"));
		}
	}
}

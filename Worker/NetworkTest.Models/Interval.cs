namespace NetworkTest.Models;

public record Interval(Interval.Units Unit, double Count)
{
	public DateTime Next => GetUpcoming().First();
	public DateTime Previous => GetExpired().First();

	internal static Func<DateTime> GetUtcNow { get; set; } = () => DateTime.UtcNow;

	public IEnumerable<DateTime> GetExpired()
	{
		var now = GetUtcNow();
		var scale = GetScale();
		var index = (long)Math.Floor(now.Ticks / scale);
		while (true)
		{
			var ticks = (long)(index * scale);
			yield return new(ticks, DateTimeKind.Utc);
			index--;
		}
	}

	public IEnumerable<DateTime> GetUpcoming()
	{
		var now = GetUtcNow();
		var scale = GetScale();
		var index = (long)Math.Ceiling(now.Ticks / scale);
		while (true)
		{
			var ticks = (long)(index * scale);
			yield return new(ticks, DateTimeKind.Utc);
			index++;
		}
	}

	private double GetScale()
	{
		return Count * Unit switch
		{
			Units.Day => TimeSpan.TicksPerDay,
			Units.Hour => TimeSpan.TicksPerHour,
			Units.Millisecond => TimeSpan.TicksPerMillisecond,
			Units.Minute => TimeSpan.TicksPerMinute,
			Units.Second => TimeSpan.TicksPerSecond,
			_ => throw new ArgumentOutOfRangeException(nameof(Unit), Unit, $"unexpected {nameof(Unit)} value: {Unit}"),
		};
	}

	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "synonyms")]
	public enum Units : byte
	{
		None = 0,
		Day = 1,
		Days = 1,
		Hour = 2,
		Hours = 2,
		Millisecond = 4,
		Milliseconds = 4,
		Minute = 8,
		Minutes = 8,
		Second = 16,
		Seconds = 16,
	}
}

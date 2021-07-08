namespace TimeSense.Models
{
    public class SensedTimeInput
    {
	[JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset TargetTime { get; set; }

	[JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset StartTime { get; set; }

	[JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset StopTime { get; set; }
    }
}

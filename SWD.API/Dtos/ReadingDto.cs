namespace SWD.API.Dtos
{
    public class ReadingDto
    {
        public long ReadingId { get; set; }
        public int SensorId { get; set; }
        public string? SensorName { get; set; }
        public string? SensorTypeName { get; set; }
        public double Value { get; set; }
        public DateTime? RecordedAt { get; set; }
    }
}

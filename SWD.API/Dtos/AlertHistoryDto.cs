namespace SWD.API.Dtos
{
    public class AlertHistoryDto
    {
        public long HistoryId { get; set; }
        public int RuleId { get; set; }
        public string? RuleName { get; set; }
        public int SensorId { get; set; }
        public string? SensorName { get; set; }
        public DateTime? TriggeredAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public double? ValueAtTrigger { get; set; }
        public string? Severity { get; set; }
        public string? Message { get; set; }
    }
}

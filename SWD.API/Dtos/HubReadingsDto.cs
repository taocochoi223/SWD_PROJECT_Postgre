
using System.Collections.Generic;

namespace SWD.API.Dtos
{
    public class HubReadingsDto
    {
        public int HubId { get; set; }
        public string Name { get; set; } = null!;
        public string MacAddress { get; set; } = null!;
        public List<SensorReadingDto> Sensors { get; set; } = new();
    }

    public class SensorReadingDto
    {
        public int SensorId { get; set; }
        public string Name { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public List<ReadingValueDto> Readings { get; set; } = new();
    }

    public class ReadingValueDto
    {
        public System.DateTime RecordedAt { get; set; }
        public float Value { get; set; }
    }
}

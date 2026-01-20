using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class Reading
{
    public long ReadingId { get; set; }

    public int SensorId { get; set; }

    public double Value { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual Sensor Sensor { get; set; } = null!;
}

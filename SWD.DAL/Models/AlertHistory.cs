using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class AlertHistory
{
    public long HistoryId { get; set; }

    public int RuleId { get; set; }

    public int SensorId { get; set; }

    public DateTime? TriggeredAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public double? ValueAtTrigger { get; set; }

    public string? Severity { get; set; }

    public string? Message { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual AlertRule Rule { get; set; } = null!;

    public virtual Sensor Sensor { get; set; } = null!;
}

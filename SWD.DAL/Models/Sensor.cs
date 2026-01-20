using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class Sensor
{
    public int SensorId { get; set; }

    public int HubId { get; set; }

    public int TypeId { get; set; }

    public string? Name { get; set; }

    public double? CurrentValue { get; set; }

    public DateTime? LastUpdate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AlertHistory> AlertHistories { get; set; } = new List<AlertHistory>();

    public virtual ICollection<AlertRule> AlertRules { get; set; } = new List<AlertRule>();

    public virtual Hub Hub { get; set; } = null!;

    public virtual ICollection<Reading> Readings { get; set; } = new List<Reading>();

    public virtual SensorType Type { get; set; } = null!;
}

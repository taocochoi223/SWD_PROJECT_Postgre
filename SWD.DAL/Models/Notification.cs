using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class Notification
{
    public long NotiId { get; set; }

    public long HistoryId { get; set; }

    public int UserId { get; set; }

    public string? Message { get; set; }

    public DateTime? SentAt { get; set; }

    public bool? IsRead { get; set; }

    public virtual AlertHistory History { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

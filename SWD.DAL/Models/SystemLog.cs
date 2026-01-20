using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class SystemLog
{
    public long LogId { get; set; }

    public string? Source { get; set; }

    public string? RawPayload { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? CreatedAt { get; set; }
}

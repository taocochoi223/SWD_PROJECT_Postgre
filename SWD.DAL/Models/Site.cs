using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class Site
{
    public int SiteId { get; set; }

    public int OrgId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? GeoLocation { get; set; }

    public virtual ICollection<Hub> Hubs { get; set; } = new List<Hub>();

    public virtual Organization Org { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

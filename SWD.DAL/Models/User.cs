using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class User
{
    public int UserId { get; set; }

    public int OrgId { get; set; }

    public int? SiteId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Organization Org { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual Site? Site { get; set; }
}

using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Ngdu
{
    public long NgduId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();

    public virtual ICollection<Workshop> Workshops { get; } = new List<Workshop>();
}

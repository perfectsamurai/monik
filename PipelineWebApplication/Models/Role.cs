using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Role
{
    public long RoleId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Advice> Advices { get; } = new List<Advice>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}

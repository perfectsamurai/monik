using System;
using System.Collections.Generic;

namespace monitoring;

public partial class User
{
    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public long? RoleId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public long? NgduId { get; set; }

    public virtual ICollection<Dynamogram> Dynamograms { get; } = new List<Dynamogram>();

    public virtual Ngdu? Ngdu { get; set; }

    public virtual Role? Role { get; set; }
}

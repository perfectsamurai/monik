using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Workshop
{
    public long WorkshopId { get; set; }

    public string? Name { get; set; }

    public long? NgduId { get; set; }

    public virtual Ngdu? Ngdu { get; set; }

    public virtual ICollection<Well> Wells { get; } = new List<Well>();
}

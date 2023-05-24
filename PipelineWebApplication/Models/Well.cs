using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Well
{
    public long WellId { get; set; }

    public string? Name { get; set; }

    public long? WorkshopId { get; set; }

    public string? TypeNp { get; set; }

    public virtual ICollection<Dynamogram> Dynamograms { get; } = new List<Dynamogram>();

    public virtual ICollection<Guide> Guides { get; } = new List<Guide>();

    public virtual Workshop? Workshop { get; set; }
}

using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Advice
{
    public long AdviceId { get; set; }

    public long? AdviceTextId { get; set; }

    public long? DynamogramId { get; set; }

    public long? RoleId { get; set; }

    public virtual AdviceText? AdviceText { get; set; }

    public virtual Dynamogram? Dynamogram { get; set; }


    public virtual Role? Role { get; set; }

}

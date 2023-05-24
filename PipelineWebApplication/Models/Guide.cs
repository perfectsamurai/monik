using System;
using System.Collections.Generic;

namespace monitoring;

public partial class Guide
{
    public long GuideId { get; set; }

    public long? WellId { get; set; }

    public int VarQ { get; set; }

    public int VarPmax { get; set; }

    public int VarPmin { get; set; }

    public int VarN { get; set; }

    public int VarL { get; set; }

    public int VarKpod { get; set; }

    public int VarKnap { get; set; }

    public int VarG { get; set; }

    public int MaxGb { get; set; }

    public virtual Well? Well { get; set; }
}

using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class Balance
{
    public int Id { get; set; }

    public int Resourcesid { get; set; }

    public int Measureid { get; set; }

    public int Countresources { get; set; }

    public virtual Measurе Measure { get; set; } = null!;

    public virtual Resource Resources { get; set; } = null!;
}

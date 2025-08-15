using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class ShippingResource
{
    public int Id { get; set; }

    public int ResourcesId { get; set; }

    public int MeasureId { get; set; }

    public int DocumentId { get; set; }

    public int Count { get; set; }

    public virtual ShippingDocument? Document { get; set; } = null!;

    public virtual Measurе? Measure { get; set; } = null!;

    public virtual Resource? Resources { get; set; } = null!;
}

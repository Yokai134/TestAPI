using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class ReceiptResource
{
    public int Id { get; set; }

    public int Resourcesid { get; set; }

    public int Measureid { get; set; }

    public int Documentid { get; set; }

    public int Countresources { get; set; }

    public virtual ReceiptDocumet? Document { get; set; }

    public virtual Measurе? Measure { get; set; }

    public virtual Resource? Resources { get; set; }
}

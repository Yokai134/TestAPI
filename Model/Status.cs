using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class Status
{
    public int Id { get; set; }

    public string Statusname { get; set; } = null!;

    public virtual ICollection<ShippingDocument> ShippingDocuments { get; set; } = new List<ShippingDocument>();
}

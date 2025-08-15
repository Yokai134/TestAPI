using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class ShippingDocument
{
    public int Id { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public int ClientId { get; set; }

    public DateOnly Date { get; set; }

    public int? StatusId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<ShippingResource> ShippingResources { get; set; } = new List<ShippingResource>();

    public virtual Status? Status { get; set; }
}

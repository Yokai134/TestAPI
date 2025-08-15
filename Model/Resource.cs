using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class Resource
{
    public int Id { get; set; }

    public string Productname { get; set; } = null!;

    public bool Isdeleted { get; set; }

    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();

    public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();

    public virtual ICollection<ShippingResource> ShippingResources { get; set; } = new List<ShippingResource>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestTaskAPI.Model;

public partial class Client
{
    public int Id { get; set; }
   
    public string Clientname { get; set; } = null!;
   
    public string Address { get; set; } = null!;

    public bool Isdeleted { get; set; }

    public virtual ICollection<ShippingDocument> ShippingDocuments { get; set; } = new List<ShippingDocument>();
}

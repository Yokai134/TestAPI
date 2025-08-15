using System;
using System.Collections.Generic;

namespace TestTaskAPI.Model;

public partial class ReceiptDocumet
{
    public int Id { get; set; }

    public string Numberdocument { get; set; } = null!;

    public DateOnly Date { get; set; }

    public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
}

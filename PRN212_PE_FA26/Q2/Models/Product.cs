using System;
using System.Collections.Generic;

namespace Q2.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int LookupId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly WorkDate { get; set; }

    public bool IsActive { get; set; }

    public virtual Category Lookup { get; set; } = null!;
}

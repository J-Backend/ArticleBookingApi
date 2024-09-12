using System;
using System.Collections.Generic;

namespace api_layaway.Models;

public partial class Layaway
{
    public int LayawayId { get; set; }

    public DateTime Opening { get; set; }

    public DateTime Closing { get; set; }

    public string State { get; set; } = null!;

    public decimal Total { get; set; }

    public int CustomerId { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<TransactionRecord> TransactionRecords { get; set; } = new List<TransactionRecord>();
}

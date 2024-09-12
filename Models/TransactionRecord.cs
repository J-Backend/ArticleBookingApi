using System;
using System.Collections.Generic;

namespace api_layaway.Models;

public partial class TransactionRecord
{
    public int TransactionRecordId { get; set; }

    public DateTime Date { get; set; }

    public decimal Payment { get; set; }

    public decimal Balance { get; set; }

    public int LayawayId { get; set; }

    public int Status { get; set; }

    public virtual Layaway Layaway { get; set; } = null!;
}

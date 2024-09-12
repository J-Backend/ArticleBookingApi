using System;
using System.Collections.Generic;

namespace api_layaway.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public decimal PayableAmount { get; set; }

    public int CustomerId { get; set; }

    public int Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}

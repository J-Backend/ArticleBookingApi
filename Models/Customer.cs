using System;
using System.Collections.Generic;

namespace api_layaway.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Layaway> Layaways { get; set; } = new List<Layaway>();
}

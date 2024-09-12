using System;
using System.Collections.Generic;

namespace api_layaway.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }

    public int Status { get; set; }

    public int LayawayId { get; set; }

    public virtual Layaway Layaway { get; set; } = null!;
}

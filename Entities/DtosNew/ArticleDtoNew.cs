using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.DtosNew
{
    public class ArticleDtoNew
    {
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

    }
}
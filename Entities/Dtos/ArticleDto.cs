using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Dtos
{
    public class ArticleDto
    {
        public int ArticleId { get; set; }

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int LayawayId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Dtos
{
    public class LayawayDto
    {
        public int LayawayId { get; set; }

        public DateTime Opening { get; set; }

        public DateTime Closing { get; set; }

        public string State { get; set; } 

        public decimal Total { get; set; }

        public int CustomerId { get; set; }

        public List<ArticleDto> Articles { get; set; }
    }
}
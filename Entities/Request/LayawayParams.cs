using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Request
{
    public class LayawayParams:PaginationParams
    {
        public int CustomerId { get; set; }
    }
}
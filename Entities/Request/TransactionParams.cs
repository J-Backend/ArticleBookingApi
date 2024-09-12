using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Request;

namespace api_layaway.Entities.Request
{
    public class TransactionParams:PaginationParams
    {

        public int LayawayId { get; set; }
    }
}
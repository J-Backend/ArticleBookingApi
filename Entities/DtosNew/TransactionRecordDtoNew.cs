using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.DtosNew
{
    public class TransactionRecordDtoNew
    {
        

        public decimal Payment { get; set; }

        public int LayawayId { get; set; }
    }
}
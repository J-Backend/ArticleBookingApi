using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Dtos
{
    public class TransactionRecordDto
    {
        public int TransactionRecordId { get; set; }

        public DateTime Date { get; set; }

        public decimal Payment { get; set; }

        public decimal Balance { get; set; }

    }
}
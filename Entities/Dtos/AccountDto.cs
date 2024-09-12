using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Dtos
{
    public class AccountDto
    {
         public int AccountId { get; set; }

         public decimal PayableAmount { get; set; }

    }
}
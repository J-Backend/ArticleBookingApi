using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.DtosNew
{
    public class LayawayDtoNew
    {
        public int CustomerId { get; set; }
        public List<ArticleDtoNew> Articles { get; set; }
    }
}
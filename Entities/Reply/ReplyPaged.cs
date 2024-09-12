using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_layaway.Entities.Reply
{
    public class ReplyPaged<T>
    {
        public T Data { get; set; }

        public int TotalRecords { get; set; }
    }
}